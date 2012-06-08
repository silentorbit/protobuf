using System;

namespace ProtocolBuffers
{
    static class MessageSerializer
    {
        public static void GenerateClassSerializer(ProtoMessage m, CodeWriter cw)
        {
            if (m.OptionExternal || m.OptionType == "interface")
            {
                //Don't make partial class of external classes or interfaces
                //Make separate static class for them
                cw.Bracket(m.OptionAccess + " static class " + m.SerializerType);
            } else
            {
                cw.Bracket(m.OptionAccess + " partial " + m.OptionType + " " + m.SerializerType);
            }

            GenerateReader(m, cw);

            GenerateWriter(m, cw);
            foreach (ProtoMessage sub in m.Messages)
            {
                cw.WriteLine();
                GenerateClassSerializer(sub, cw);
            }
            cw.EndBracket();
            cw.WriteLine();
            return;
        }
        
        static void GenerateReader(ProtoMessage m, CodeWriter cw)
        {
            #region Helper Deserialize Methods
            string refstr = (m.OptionType == "struct") ? "ref " : "";
            if (m.OptionType != "interface")
            {
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(Stream stream)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("Deserialize(stream, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();
            
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(byte[] buffer)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
                cw.WriteLine("Deserialize(ms, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();
            }

            cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " Deserialize(byte[] buffer, " + refstr + m.FullCsType + " instance)");
            cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
            cw.WriteIndent("Deserialize(ms, " + refstr + "instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            #endregion

            string[] methods = new string[]{
                "Deserialize", //Default old one
                "DeserializeLengthDelimited", //Start by reading length prefix and stay within that limit
            };

            //Main Deserialize
            foreach (string method in methods)
            {
                if (method == "Deserialize")
                    cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " " + method + "(Stream stream, " + refstr + m.FullCsType + " instance)");
                else
                    cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " " + method + "(Stream stream, " + refstr + m.FullCsType + " instance)");

                if (IsUsingBinaryWriter(m))
                    cw.WriteLine("BinaryReader br = new BinaryReader(stream);");

                //Prepare List<> and default values
                foreach (Field f in m.Fields.Values)
                {
                    if (f.Rule == FieldRule.Repeated)
                    {
                        cw.WriteLine("if (instance." + f.Name + " == null)");
                        cw.WriteIndent("instance." + f.Name + " = new List<" + f.ProtoType.FullCsType + ">();");
                    } else if (f.OptionDefault != null)
                    {
                        if (f.ProtoType is ProtoEnum)
                            cw.WriteLine("instance." + f.Name + " = " + f.ProtoType.FullCsType + "." + f.OptionDefault + ";");
                        else
                            cw.WriteLine("instance." + f.Name + " = " + f.OptionDefault + ";");
                    } else if (f.Rule == FieldRule.Optional)
                    {
                        if (f.ProtoType is ProtoEnum)
                        {
                            ProtoEnum pe = f.ProtoType as ProtoEnum;
                            //the default value is the first value listed in the enum's type definition
                            foreach (var kvp in pe.Enums)
                            {
                                cw.WriteLine("instance." + f.Name + " = " + kvp.Key + ";");
                                break;
                            }
                        }
                    }
                }

                if (method == "DeserializeLengthDelimited")
                {
                    //Important to read stream position after we have read the length field
                    cw.WriteLine("long limit = ProtocolParser.ReadUInt32(stream);");
                    cw.WriteLine("limit += stream.Position;");
                }

                cw.WhileBracket("true");

                if (method == "DeserializeLengthDelimited")
                {
                    cw.IfBracket("stream.Position >= limit");
                    cw.WriteLine("if(stream.Position == limit)");
                    cw.WriteIndent("break;");
                    cw.WriteLine("else");
                    cw.WriteIndent("throw new InvalidOperationException(\"Read past max limit\");");
                    cw.EndBracket();
                }

                cw.WriteLine("ProtocolBuffers.Key key = null;");
                cw.WriteLine("int keyByte = stream.ReadByte();");
                cw.WriteLine("if (keyByte == -1)");
                if (method == "Deserialize")
                    cw.WriteIndent("break;");
                else
                    cw.WriteIndent("throw new System.IO.EndOfStreamException();");

                cw.Comment("Optimized reading of known fields with field ID < 16");
                cw.Switch("keyByte");
                foreach (Field f in m.Fields.Values)
                {
                    if (f.ID >= 16)
                        continue;
                    cw.Comment("Field " + f.ID + " " + f.WireType);
                    cw.Case(((f.ID << 3) | (int)f.WireType));
                    FieldSerializer.GenerateFieldReader(f, cw);
                    cw.WriteLine("break;");
                }
                cw.CaseDefault();
                cw.WriteLine("key = ProtocolParser.ReadKey((byte)keyByte, stream);");
                cw.WriteLine("break;");
                cw.EndBracket();
                cw.WriteLine();

                cw.WriteLine("if (key == null)");
                cw.WriteIndent("continue;");
                cw.WriteLine();

                cw.Comment("Reading field ID > 16 and unknown field ID/wire type combinations");
                cw.Switch("key.Field");
                cw.Case(0);
                cw.WriteLine("throw new InvalidDataException(\"Invalid field id: 0, something went wrong in the stream\");");
                foreach (Field f in m.Fields.Values)
                {
                    if (f.ID < 16)
                        continue;
                    cw.Case(f.ID);
                    //Makes sure we got the right wire type
                    cw.WriteLine("if(key.WireType != Wire." + f.WireType + ")");
                    cw.WriteIndent("break;");
                    FieldSerializer.GenerateFieldReader(f, cw);
                    cw.WriteLine("continue;");
                }
                cw.CaseDefault();
                if (m.OptionPreserveUnknown)
                {
                    cw.WriteLine("if (instance.PreservedFields == null)");
                    cw.WriteIndent("instance.PreservedFields = new List<KeyValue>();");
                    cw.WriteLine("instance.PreservedFields.Add(new KeyValue(key, ProtocolParser.ReadValueBytes(stream, key)));");
                } else
                {
                    cw.WriteLine("ProtocolParser.SkipKey(stream, key);");
                }
                cw.WriteLine("break;");
                cw.EndBracket();
                cw.EndBracket();
                cw.WriteLine();

                if (m.OptionTriggers)
                    cw.WriteLine("instance.AfterDeserialize();");
                cw.WriteLine("return instance;");
                cw.EndBracket();
                cw.WriteLine();
            }

            return;
        }

        /// <summary>
        /// Generates code for writing a class/message
        /// </summary>
        static void GenerateWriter(ProtoMessage m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " static void Serialize(Stream stream, " + m.CsType + " instance)");
            if (m.OptionTriggers)
            {
                cw.WriteLine("instance.BeforeSerialize();");
                cw.WriteLine();
            }
            if (IsUsingBinaryWriter(m))
                cw.WriteLine("BinaryWriter bw = new BinaryWriter(stream);");
            
            foreach (Field f in m.Fields.Values)
                FieldSerializer.GenerateFieldWriter(m, f, cw);

            if (m.OptionPreserveUnknown)
            {
                cw.IfBracket("instance.PreservedFields != null");
                cw.ForeachBracket("KeyValue kv in instance.PreservedFields");
                cw.WriteLine("ProtocolParser.WriteKey(stream, kv.Key);");
                cw.WriteLine("stream.Write(kv.Value, 0, kv.Value.Length);");
                cw.EndBracket();
                cw.EndBracket();
            }
            cw.EndBracket();
            cw.WriteLine();

            cw.Bracket(m.OptionAccess + " static byte[] SerializeToBytes(" + m.CsType + " instance)");
            cw.Using("MemoryStream ms = new MemoryStream()");
            cw.WriteLine("Serialize(ms, instance);");
            cw.WriteLine("return ms.ToArray();");
            cw.EndBracket();
            cw.EndBracket();
        }

        /// <summary>
        /// Determines if a BinaryWriter will be used
        /// </summary>
        static bool IsUsingBinaryWriter(ProtoMessage m)
        {
            foreach (Field f in m.Fields.Values)
            {
                if (f.ProtoType.WireType == Wire.Fixed32)
                    return true;
                if (f.ProtoType.WireType == Wire.Fixed64)
                    return true;
            }
            return false;
        }
    }
}

