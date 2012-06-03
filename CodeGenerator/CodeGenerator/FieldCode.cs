using System;

namespace ProtocolBuffers
{
    static class FieldCode
    {
        #region Reader
        
        public static void GenerateFieldReader(Field f, CodeWriter cw)
        {
            if (f.Rule == FieldRule.Repeated)
            {
                //Make sure we are not reading a list of interfaces
                if (f.ProtoTypeMessage != null && f.ProtoTypeMessage.OptionType == "interface")
                {
                    cw.WriteLine("throw new InvalidOperationException(\"Can't deserialize a list of interfaces\");");
                    return;
                }

                if (f.OptionPacked == true)
                {
                    cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream(ProtocolParser.ReadBytes(stream))");
                    cw.WhileBracket("true");
                    cw.WriteLine("if (ms" + f.ID + ".Position == ms" + f.ID + ".Length)");
                    cw.WriteIndent("break;");
                    cw.WriteLine("instance." + f.Name + ".Add(" + GenerateFieldTypeReader(f, "ms" + f.ID, "br", null) + ");");
                    cw.EndBracket();
                    cw.EndBracket();
                } else
                {
                    cw.WriteLine("instance." + f.Name + ".Add(" + GenerateFieldTypeReader(f, "stream", "br", null) + ");");
                }
            } else
            {           
                if (f.ProtoType == ProtoTypes.Message)
                {
                    if (f.OptionReadOnly)
                        cw.WriteLine(GenerateFieldTypeReader(f, "stream", "br", "instance." + f.Name) + ";");
                    else
                    {
                        if (f.ProtoTypeMessage.OptionType == "struct")
                        {
                            cw.WriteLine(GenerateFieldTypeReader(f, "stream", "br", "ref instance." + f.Name) + ";");
                            return;
                        }

                        cw.WriteLine("if (instance." + f.Name + " == null)");
                        if (f.ProtoTypeMessage.OptionType == "interface")
                            cw.WriteIndent("throw new InvalidOperationException(\"Can't deserialize into a interfaces null pointer\");");
                        else
                            cw.WriteIndent("instance." + f.Name + " = " + GenerateFieldTypeReader(f, "stream", "br", null) + ";");
                        cw.WriteLine("else");
                        cw.WriteIndent(GenerateFieldTypeReader(f, "stream", "br", "instance." + f.Name) + ";");
                    }
                } else
                    cw.WriteLine("instance." + f.Name + " = " + GenerateFieldTypeReader(f, "stream", "br", "instance." + f.Name) + ";");
            }
        }

        static string GenerateFieldTypeReader(Field f, string stream, string binaryReader, string instance)
        {
            if (f.OptionCodeType != null)
            {
                switch (f.OptionCodeType)
                {
                    case "DateTime":
                        return "new DateTime((long)ProtocolParser.ReadUInt64(" + stream + "))";
                    case "TimeSpan":
                        return "new TimeSpan((long)ProtocolParser.ReadUInt64(" + stream + "))";
                    default:
                    //Assume enum
                        return "(" + f.OptionCodeType + ")" + GenerateFieldTypeReaderPrimitive(f, stream, instance);
                }
            }
            
            return GenerateFieldTypeReaderPrimitive(f, stream, instance);
        }

        static string GenerateFieldTypeReaderPrimitive(Field f, string stream, string instance)
        {
            switch (f.ProtoType)
            {
                case ProtoTypes.Double:
                    return "br.ReadDouble()";
                case ProtoTypes.Float:
                    return "br.ReadSingle()";
                case ProtoTypes.Fixed32:
                    return "br.ReadUInt32()";
                case ProtoTypes.Fixed64:
                    return "br.ReadUInt64()";
                case ProtoTypes.Sfixed32:
                    return "br.ReadInt32()";
                case ProtoTypes.Sfixed64:
                    return "br.ReadInt64()";
                case ProtoTypes.Int32:
                    return "(int)ProtocolParser.ReadUInt32(" + stream + ")";
                case ProtoTypes.Int64:
                    return "(long)ProtocolParser.ReadUInt64(" + stream + ")";
                case ProtoTypes.Uint32:
                    return "ProtocolParser.ReadUInt32(" + stream + ")";
                case ProtoTypes.Uint64:
                    return "ProtocolParser.ReadUInt64(" + stream + ")";
                case ProtoTypes.Sint32:
                    return "ProtocolParser.ReadSInt32(" + stream + ")";
                case ProtoTypes.Sint64:
                    return "ProtocolParser.ReadSInt64(" + stream + ")";
                case ProtoTypes.Bool:
                    return "ProtocolParser.ReadBool(" + stream + ")";
                case ProtoTypes.String:
                    return "ProtocolParser.ReadString(" + stream + ")";
                case ProtoTypes.Bytes:
                    return "ProtocolParser.ReadBytes(" + stream + ")";
                case ProtoTypes.Enum:
                    return "(" + f.PropertyItemType + ")ProtocolParser.ReadUInt32(" + stream + ")";
                case ProtoTypes.Message:                
                    if (f.Rule == FieldRule.Repeated || instance == null)
                        return f.FullSerializerPath + ".Deserialize(ProtocolParser.ReadBytes(" + stream + "))";
                    else
                        return f.FullSerializerPath + ".Deserialize(ProtocolParser.ReadBytes(" + stream + "), " + instance + ")";
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
        
        
        
        #region Writer
        
        /// <summary>
        /// Generates code for writing one field
        /// </summary>
        public static void GenerateFieldWriter(Message m, Field f, CodeWriter cw)
        {
            if (f.Rule == FieldRule.Repeated)
            {
                if (f.OptionPacked == true)
                {
                    cw.IfBracket("instance." + f.Name + " != null");
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                    cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream()");
                    switch (f.ProtoType)
                    {
                        case ProtoTypes.Double:
                        case ProtoTypes.Float:
                        case ProtoTypes.Fixed32:
                        case ProtoTypes.Fixed64:
                        case ProtoTypes.Sfixed32:
                        case ProtoTypes.Sfixed64:
                            cw.WriteLine("BinaryWriter bw" + f.ID + " = new BinaryWriter(ms" + f.ID + ");");
                            break;
                    }
                    
                    cw.ForeachBracket(f.PropertyItemType + " i" + f.ID + " in instance." + f.Name);
                    cw.WriteLine(GenerateFieldTypeWriter(f, "ms" + f.ID, "bw" + f.ID, "i" + f.ID));
                    cw.EndBracket();
                    cw.WriteLine("ProtocolParser.WriteBytes(stream, ms" + f.ID + ".ToArray());");
                    cw.EndBracket();
                    cw.EndBracket();
                } else
                {
                    cw.IfBracket("instance." + f.Name + " != null");
                    cw.ForeachBracket(f.PropertyItemType + " i" + f.ID + " in instance." + f.Name);
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                    cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "i" + f.ID));
                    cw.EndBracket();
                    cw.EndBracket();
                }
                return;
            } else if (f.Rule == FieldRule.Optional)
            {           
                switch (f.ProtoType)
                {
                    case ProtoTypes.String:
                    case ProtoTypes.Message:
                    case ProtoTypes.Bytes:
                        //Struct always exist, not optional
                        if(f.ProtoTypeMessage == null || f.ProtoTypeMessage.OptionType != "struct")
                            cw.IfBracket("instance." + f.Name + " != null");
                        cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                        cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                        if(f.ProtoTypeMessage == null || f.ProtoTypeMessage.OptionType != "struct")
                            cw.EndBracket();
                        return;
                    case ProtoTypes.Enum:
                        cw.IfBracket("instance." + f.Name + " != " + f.PropertyItemType + "." + f.OptionDefault);
                        cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                        cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                        cw.EndBracket();
                        return;
                    default:
                        cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                        cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                        return;
                }
            } else if (f.Rule == FieldRule.Required)
            {           
                switch (f.ProtoType)
                {
                    case ProtoTypes.String:
                    case ProtoTypes.Message:
                    case ProtoTypes.Bytes:
                        if (f.ProtoType == ProtoTypes.Message && f.ProtoTypeMessage.OptionType == "struct")
                            break;
                        cw.WriteLine("if (instance." + f.Name + " == null)");
                        cw.WriteIndent("throw new ArgumentNullException(\"" + f.Name + "\", \"Required by proto specification.\");");
                        break;
                }
                cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.WireType + "));");
                cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                return;
            }           
            throw new NotImplementedException("Unknown rule: " + f.Rule);
        }
                    
        public static string GenerateFieldTypeWriter(Field f, string stream, string binaryWriter, string instance)
        {
            if (f.OptionCodeType != null)
            {
                switch (f.OptionCodeType)
                {
                    case "DateTime":
                    case "TimeSpan":
                        return "ProtocolParser.WriteUInt64(" + stream + ",(ulong)" + instance + ".Ticks);";
                    default: //enum
                        break;
                }
            }
            
            switch (f.ProtoType)
            {
                case ProtoTypes.Double:
                case ProtoTypes.Float:
                case ProtoTypes.Fixed32:
                case ProtoTypes.Fixed64:
                case ProtoTypes.Sfixed32:
                case ProtoTypes.Sfixed64:
                    return binaryWriter + ".Write(" + instance + ");";
                case ProtoTypes.Int32:
                    return "ProtocolParser.WriteUInt32(" + stream + ",(uint)" + instance + ");";
                case ProtoTypes.Int64:
                    return "ProtocolParser.WriteUInt64(" + stream + ",(ulong)" + instance + ");";
                case ProtoTypes.Uint32:
                    return "ProtocolParser.WriteUInt32(" + stream + ", " + instance + ");";
                case ProtoTypes.Uint64:
                    return "ProtocolParser.WriteUInt64(" + stream + ", " + instance + ");";
                case ProtoTypes.Sint32:
                    return "ProtocolParser.WriteSInt32(" + stream + ", " + instance + ");";
                case ProtoTypes.Sint64:
                    return "ProtocolParser.WriteSInt64(" + stream + ", " + instance + ");";
                case ProtoTypes.Bool:
                    return "ProtocolParser.WriteBool(" + stream + ", " + instance + ");";
                case ProtoTypes.String:
                    return "ProtocolParser.WriteString(" + stream + ", " + instance + ");";
                case ProtoTypes.Bytes:
                    return "ProtocolParser.WriteBytes(" + stream + ", " + instance + ");";
                case ProtoTypes.Enum:
                    return "ProtocolParser.WriteUInt32(" + stream + ",(uint)" + instance + ");";
                case ProtoTypes.Message: 
                    CodeWriter cw = new CodeWriter();
                    cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream()");
                    cw.WriteLine(f.FullSerializerPath + ".Serialize(ms" + f.ID + ", " + instance + ");");
                    cw.WriteLine("ProtocolParser.WriteBytes(" + stream + ", ms" + f.ID + ".ToArray());");
                    cw.EndBracket();
                    return cw.Code;
                default:
                    throw new NotImplementedException();
            }
        }
        
        #endregion
    }
}

