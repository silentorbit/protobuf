using System;

namespace ProtocolBuffers
{
    static class SerializerCode
    {
        public static void GenerateClassSerializer(Message m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " partial class " + m.CSType);

            GenerateReader(m, cw);

            GenerateWriter(m, cw);
            foreach (Message sub in m.Messages)
            {
                cw.WriteLine();
                GenerateClassSerializer(sub, cw);
            }
            cw.EndBracket();
            cw.WriteLine();
            return;
        }
        
        public static void GenerateGenericClassSerializer(Message m, CodeWriter cw)
        {
            GenerateGenericReader(m, cw);
            cw.WriteLine();
            GenerateGenericWriter(m, cw);
            foreach (Message sub in m.Messages)
            {
                cw.WriteLine();
                GenerateGenericClassSerializer(sub, cw);
            }
            return;
        }
        
        static void GenerateReader(Message m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " static " + m.CSType + " Deserialize(Stream stream)");
            cw.WriteLine("" + m.CSType + " instance = new " + m.CSType + "();");
            cw.WriteLine("Deserialize(stream, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            
            cw.Bracket(m.OptionAccess + " static " + m.CSType + " Deserialize(byte[] buffer)");
            cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
            cw.WriteIndent("return Deserialize(ms);");
            cw.EndBracketSpace();
            
            cw.Bracket(m.OptionAccess + " static T Deserialize<T>(Stream stream) where T : " + m.FullCSType + ", new()");
            cw.WriteLine("T instance = new T();");
            cw.WriteLine("Deserialize(stream, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            
            cw.Bracket(m.OptionAccess + " static T Deserialize<T>(byte[] buffer) where T : " + m.FullCSType + ", new()");
            cw.WriteLine("T instance = new T();");
            cw.WriteLine("Deserialize(buffer, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            
            cw.Bracket(m.OptionAccess + " static " + m.FullCSType + " Deserialize(byte[] buffer, " + m.FullCSType + " instance)");
            cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
            cw.WriteIndent("Deserialize(ms, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();
            
            cw.Bracket(m.OptionAccess + " static " + m.FullCSType + " Deserialize(Stream stream, " + m.FullCSType + " instance)");
            if (IsUsingBinaryWriter(m))
                cw.WriteLine("BinaryReader br = new BinaryReader(stream);");

            foreach (Field f in m.Fields.Values)
            {
                if (f.Rule == FieldRule.Repeated)
                {
                    cw.WriteLine("if (instance." + f.Name + " == null)");
                    cw.WriteIndent("instance." + f.Name + " = new List<" + f.PropertyItemType + ">();");
                } else if (f.OptionDefault != null)
                {
                    if (f.ProtoType == ProtoTypes.Enum)
                        cw.WriteLine("instance." + f.Name + " = " + f.FullPath + "." + f.OptionDefault + ";");
                    else
                        cw.WriteLine("instance." + f.Name + " = " + f.OptionDefault + ";");
                } else if (f.Rule == FieldRule.Optional)
                {
                    if (f.ProtoType == ProtoTypes.Enum)
                    {
                        //the default value is the first value listed in the enum's type definition
                        foreach (var kvp in f.ProtoTypeEnum.Enums)
                        {
                            cw.WriteLine("instance." + f.Name + " = " + kvp.Key + ";");
                            break;
                        }
                    }
                }
            }

            cw.WhileBracket("true");
            cw.WriteLine("ProtocolBuffers.Key key = null;");
            cw.WriteLine("int keyByte = stream.ReadByte();");
            cw.WriteLine("if (keyByte == -1)");
            cw.WriteIndent("break;");

            cw.Comment("Optimized reading of known fields with field ID < 16");
            cw.Switch("keyByte");
            foreach (Field f in m.Fields.Values)
            {
                if (f.ID >= 16)
                    continue;
                cw.Comment("Field " + f.ID + " " + f.WireType);
                cw.Case(((f.ID << 3) | (int)f.WireType));
                FieldCode.GenerateFieldReader(f, cw);
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
                FieldCode.GenerateFieldReader(f, cw);
                cw.WriteLine("break;");
            }
            cw.CaseDefault();
            if (m.OptionPreserveUnknown)
            {
                cw.WriteLine("if(instance.PreservedFields == null)");
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

            cw.Bracket(m.OptionAccess + " static " + m.FullCSType + " Read(byte[] buffer, " + m.FullCSType + " instance)");
            cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
            cw.WriteIndent("Deserialize(ms, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracket();

            cw.WriteLine();
            return;
        }

        static void GenerateGenericReader(Message m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " static " + m.FullCSType + " Read(Stream stream, " + m.FullCSType + " instance)");
            cw.WriteLine("return " + m.FullCSType + ".Deserialize(stream, instance);");
            cw.EndBracket();
            cw.WriteLine();
            cw.Bracket(m.OptionAccess + " static " + m.FullCSType + " Read(byte[] buffer, " + m.FullCSType + " instance)");
            cw.WriteLine("using (MemoryStream ms = new MemoryStream(buffer))");
            cw.WriteIndent(m.FullCSType + ".Deserialize(ms, instance);");
            cw.WriteLine("return instance;");
            cw.EndBracket();
        }
        
        
        /// <summary>
        /// Generates code for writing a class/message
        /// </summary>
        static void GenerateWriter(Message m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " static void Serialize(Stream stream, " + m.CSType + " instance)");
            if (m.OptionTriggers)
            {
                cw.WriteLine("instance.BeforeSerialize();");
                cw.WriteLine();
            }
            if (IsUsingBinaryWriter(m))
                cw.WriteLine("BinaryWriter bw = new BinaryWriter(stream);");
            
            foreach (Field f in m.Fields.Values)
                FieldCode.GenerateFieldWriter(m, f, cw);

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

            cw.Bracket(m.OptionAccess + " static byte[] SerializeToBytes(" + m.CSType + " instance)");
            cw.Using("MemoryStream ms = new MemoryStream()");
            cw.WriteLine("Serialize(ms, instance);");
            cw.WriteLine("return ms.ToArray();");
            cw.EndBracket();
            cw.EndBracket();
        }
        
        /// <summary>
        /// Generates code for writing a class as a protobuf message
        /// </summary>
        static void GenerateGenericWriter(Message m, CodeWriter cw)
        {
            cw.Bracket(m.OptionAccess + " static void Write(Stream stream, " + m.FullCSType + " instance)");
            cw.WriteLine("" + m.FullCSType + ".Serialize(stream, instance);");
            cw.EndBracket();
        }
        
        /// <summary>
        /// Determines if a BinaryWriter will be used
        /// </summary>
        static bool IsUsingBinaryWriter(Message m)
        {
            foreach (Field f in m.Fields.Values)
            {
                if (f.WireType == Wire.Fixed32 || f.WireType == Wire.Fixed64)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

