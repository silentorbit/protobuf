using System;
using SilentOrbit.Code;

namespace SilentOrbit.ProtocolBuffers
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
            }
            else
            {
                cw.Attribute("System.Serializable()");
                cw.Bracket(m.OptionAccess + " partial " + m.OptionType + " " + m.SerializerType);
            }

            GenerateReader(m, cw);

            GenerateWriter(m, cw);
            foreach (ProtoMessage sub in m.Messages.Values)
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
                cw.Summary("Helper: create a new instance to deserializing into");
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(Stream stream)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("Deserialize(stream, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();

                cw.Summary("Helper: create a new instance to deserializing into");
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " DeserializeLengthDelimited(Stream stream)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("DeserializeLengthDelimited(stream, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();

                cw.Summary("Helper: create a new instance to deserializing into");
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " DeserializeLength(Stream stream, int length)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("DeserializeLength(stream, length, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();

                cw.Summary("Helper: put the buffer into a MemoryStream and create a new instance to deserializing into");
                cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(byte[] buffer)");
                cw.WriteLine(m.CsType + " instance = new " + m.CsType + "();");
                cw.WriteLine("using (var ms = new MemoryStream(buffer))");
                cw.WriteIndent("Deserialize(ms, " + refstr + "instance);");
                cw.WriteLine("return instance;");
                cw.EndBracketSpace();
            }

            cw.Summary("Helper: put the buffer into a MemoryStream before deserializing");
            cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " Deserialize(byte[] buffer, " + refstr + m.FullCsType + " instance)");
            cw.WriteLine("using (var ms = new MemoryStream(buffer))");
            cw.WriteIndent("Deserialize(ms, " + refstr + "instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            #endregion

            string[] methods = new string[]
            {
                "Deserialize", //Default old one
                "DeserializeLengthDelimited", //Start by reading length prefix and stay within that limit
                "DeserializeLength", //Read at most length bytes given by argument
            };

            //Main Deserialize
            foreach (string method in methods)
            {
                if (method == "Deserialize")
                {
                    cw.Summary("Takes the remaining content of the stream and deserialze it into the instance.");
                    cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " " + method + "(Stream stream, " + refstr + m.FullCsType + " instance)");
                }
                else if (method == "DeserializeLengthDelimited")
                {
                    cw.Summary("Read the VarInt length prefix and the given number of bytes from the stream and deserialze it into the instance.");
                    cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " " + method + "(Stream stream, " + refstr + m.FullCsType + " instance)");
                }
                else if (method == "DeserializeLength")
                {
                    cw.Summary("Read the given number of bytes from the stream and deserialze it into the instance.");
                    cw.Bracket(m.OptionAccess + " static " + m.FullCsType + " " + method + "(Stream stream, int length, " + refstr + m.FullCsType + " instance)");
                }
                else
                    throw new NotImplementedException();

                if (m.IsUsingBinaryWriter)
                    cw.WriteLine("BinaryReader br = new BinaryReader(stream);");

                //Prepare List<> and default values
                foreach (Field f in m.Fields.Values)
                {
                    if (f.Rule == FieldRule.Repeated)
                    {
                        cw.WriteLine("if (instance." + f.CsName + " == null)");
                        cw.WriteIndent("instance." + f.CsName + " = new List<" + f.ProtoType.FullCsType + ">();");
                    }
                    else if (f.OptionDefault != null)
                    {
                        if (f.ProtoType is ProtoEnum)
                            cw.WriteLine("instance." + f.CsName + " = " + f.ProtoType.FullCsType + "." + f.OptionDefault + ";");
                        else
                            cw.WriteLine("instance." + f.CsName + " = " + f.OptionDefault + ";");
                    }
                    else if (f.Rule == FieldRule.Optional)
                    {
                        if (f.ProtoType is ProtoEnum)
                        {
                            ProtoEnum pe = f.ProtoType as ProtoEnum;
                            //the default value is the first value listed in the enum's type definition
                            foreach (var kvp in pe.Enums)
                            {
                                cw.WriteLine("instance." + f.CsName + " = " + pe.FullCsType + "." + kvp.Name + ";");
                                break;
                            }
                        }
                    }
                }

                if (method == "DeserializeLengthDelimited")
                {
                    //Important to read stream position after we have read the length field
                    cw.WriteLine("long limit = global::SilentOrbit.ProtocolBuffers.ProtocolParser.ReadUInt32(stream);");
                    cw.WriteLine("limit += stream.Position;");
                }
                if (method == "DeserializeLength")
                {
                    //Important to read stream position after we have read the length field
                    cw.WriteLine("long limit = stream.Position + length;");
                }

                cw.WhileBracket("true");

                if (method == "DeserializeLengthDelimited" || method == "DeserializeLength")
                {
                    cw.IfBracket("stream.Position >= limit");
                    cw.WriteLine("if (stream.Position == limit)");
                    cw.WriteIndent("break;");
                    cw.WriteLine("else");
                    cw.WriteIndent("throw new InvalidOperationException(\"Read past max limit\");");
                    cw.EndBracket();
                }

                cw.WriteLine("int keyByte = stream.ReadByte();");
                cw.WriteLine("if (keyByte == -1)");
                if (method == "Deserialize")
                    cw.WriteIndent("break;");
                else
                    cw.WriteIndent("throw new System.IO.EndOfStreamException();");

                //Determine if we need the lowID optimization
                bool hasLowID = false;
                foreach (Field f in m.Fields.Values)
                {
                    if (f.ID < 16)
                    {
                        hasLowID = true;
                        break;
                    }
                }

                if (hasLowID)
                {
                    cw.Comment("Optimized reading of known fields with field ID < 16");
                    cw.Switch("keyByte");
                    foreach (Field f in m.Fields.Values)
                    {
                        if (f.ID >= 16)
                            continue;
                        cw.Dedent();
                        cw.Comment("Field " + f.ID + " " + f.WireType);
                        cw.Indent();
                        cw.Case(((f.ID << 3) | (int)f.WireType));
                        if (FieldSerializer.FieldReader(f, cw))
                            cw.WriteLine("continue;");
                    }
                    cw.SwitchEnd();
                    cw.WriteLine();
                }
                cw.WriteLine("var key = global::SilentOrbit.ProtocolBuffers.ProtocolParser.ReadKey((byte)keyByte, stream);");

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
                    cw.WriteLine("if(key.WireType != global::SilentOrbit.ProtocolBuffers.Wire." + f.WireType + ")");
                    cw.WriteIndent("break;"); //This can be changed to throw an exception for unknown formats.
                    if (FieldSerializer.FieldReader(f, cw))
                        cw.WriteLine("continue;");
                }
                cw.CaseDefault();
                if (m.OptionPreserveUnknown)
                {
                    cw.WriteLine("if (instance.PreservedFields == null)");
                    cw.WriteIndent("instance.PreservedFields = new List<global::SilentOrbit.ProtocolBuffers.KeyValue>();");
                    cw.WriteLine("instance.PreservedFields.Add(new global::SilentOrbit.ProtocolBuffers.KeyValue(key, global::SilentOrbit.ProtocolBuffers.ProtocolParser.ReadValueBytes(stream, key)));");
                }
                else
                {
                    cw.WriteLine("global::SilentOrbit.ProtocolBuffers.ProtocolParser.SkipKey(stream, key);");
                }
                cw.WriteLine("break;");
                cw.SwitchEnd();
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
            cw.Summary("Serialize the instance into the stream");
            cw.Bracket(m.OptionAccess + " static void Serialize(Stream stream, " + m.CsType + " instance)");
            if (m.OptionTriggers)
            {
                cw.WriteLine("instance.BeforeSerialize();");
                cw.WriteLine();
            }
            if (m.IsUsingBinaryWriter)
                cw.WriteLine("BinaryWriter bw = new BinaryWriter(stream);");

            foreach (Field f in m.Fields.Values)
                FieldSerializer.FieldWriter(m, f, cw);

            if (m.OptionPreserveUnknown)
            {
                cw.IfBracket("instance.PreservedFields != null");
                cw.ForeachBracket("var kv in instance.PreservedFields");
                cw.WriteLine("global::SilentOrbit.ProtocolBuffers.ProtocolParser.WriteKey(stream, kv.Key);");
                cw.WriteLine("stream.Write(kv.Value, 0, kv.Value.Length);");
                cw.EndBracket();
                cw.EndBracket();
            }
            cw.EndBracket();
            cw.WriteLine();

            cw.Summary("Helper: Serialize into a MemoryStream and return its byte array");
            cw.Bracket(m.OptionAccess + " static byte[] SerializeToBytes(" + m.CsType + " instance)");
            cw.Using("var ms = new MemoryStream()");
            cw.WriteLine("Serialize(ms, instance);");
            cw.WriteLine("return ms.ToArray();");
            cw.EndBracket();
            cw.EndBracket();

            cw.Summary("Helper: Serialize with a varint length prefix");
            cw.Bracket(m.OptionAccess + " static void SerializeLengthDelimited(Stream stream, " + m.CsType + " instance)");
            cw.WriteLine("var data = SerializeToBytes(instance);");
            cw.WriteLine("global::SilentOrbit.ProtocolBuffers.ProtocolParser.WriteUInt32(stream, (uint)data.Length);");
            cw.WriteLine("stream.Write(data, 0, data.Length);");
            cw.EndBracket();
        }
    }
}

