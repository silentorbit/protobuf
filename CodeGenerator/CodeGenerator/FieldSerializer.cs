using System;

namespace ProtocolBuffers
{
    static class FieldSerializer
    {
        #region Reader
        
        public static void GenerateFieldReader(Field f, CodeWriter cw)
        {
            if (f.Rule == FieldRule.Repeated)
            {
                //Make sure we are not reading a list of interfaces
                if (f.ProtoType.OptionType == "interface")
                {
                    cw.WriteLine("throw new InvalidOperationException(\"Can't deserialize a list of interfaces\");");
                    return;
                }

                if (f.OptionPacked == true)
                {
                    cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream(ProtocolParser.ReadBytes(stream))");
                    cw.WriteLine("while(ms" + f.ID + ".Position < ms" + f.ID + ".Length)");
                    cw.WriteIndent("instance." + f.Name + ".Add(" + GenerateFieldTypeReader(f, "ms" + f.ID, "br", null) + ");");
                    cw.EndBracket();
                } else
                {
                    cw.WriteLine("instance." + f.Name + ".Add(" + GenerateFieldTypeReader(f, "stream", "br", null) + ");");
                }
            } else
            {   
                if (f.OptionReadOnly)
                {
                    //The only "readonly" fields we can modify
                    //We could possibly support bytes primitive too but it would require the incoming length to match the wire length
                    if (f.ProtoType is ProtoMessage)
                    {
                        cw.WriteLine(GenerateFieldTypeReader(f, "stream", "br", "instance." + f.Name) + ";");
                        return;
                    }
                    cw.WriteIndent("throw new InvalidOperationException(\"Can't deserialize into a readonly primitive field\");");
                    return;
                }
                
                if (f.ProtoType is ProtoMessage)
                {
                    if (f.ProtoType.OptionType == "struct")
                    {
                        cw.WriteLine(GenerateFieldTypeReader(f, "stream", "br", "ref instance." + f.Name) + ";");
                        return;
                    }

                    cw.WriteLine("if (instance." + f.Name + " == null)");
                    if (f.ProtoType.OptionType == "interface")
                        cw.WriteIndent("throw new InvalidOperationException(\"Can't deserialize into a interfaces null pointer\");");
                    else
                        cw.WriteIndent("instance." + f.Name + " = " + GenerateFieldTypeReader(f, "stream", "br", null) + ";");
                    cw.WriteLine("else");
                    cw.WriteIndent(GenerateFieldTypeReader(f, "stream", "br", "instance." + f.Name) + ";");
                    return;
                } 

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
            if (f.ProtoType is ProtoMessage)
            {
                var m = f.ProtoType as ProtoMessage;
                if (f.Rule == FieldRule.Repeated || instance == null)
                    return m.FullSerializerType + ".Deserialize(ProtocolParser.ReadBytes(" + stream + "))";
                else
                    return m.FullSerializerType + ".Deserialize(ProtocolParser.ReadBytes(" + stream + "), " + instance + ")";
            }

            if (f.ProtoType is ProtoEnum)
                return "(" + f.ProtoType.CsType + ")ProtocolParser.ReadUInt32(" + stream + ")";
            
            if (f.ProtoType is ProtoBuiltin)
            {
                switch (f.ProtoType.ProtoName)
                {
                    case ProtoBuiltin.Double:
                        return "br.ReadDouble()";
                    case ProtoBuiltin.Float:
                        return "br.ReadSingle()";
                    case ProtoBuiltin.Int32:
                        return "(int)ProtocolParser.ReadUInt32(" + stream + ")";
                    case ProtoBuiltin.Int64:
                        return "(long)ProtocolParser.ReadUInt64(" + stream + ")";
                    case ProtoBuiltin.UInt32:
                        return "ProtocolParser.ReadUInt32(" + stream + ")";
                    case ProtoBuiltin.UInt64:
                        return "ProtocolParser.ReadUInt64(" + stream + ")";
                    case ProtoBuiltin.SInt32:
                        return "ProtocolParser.ReadSInt32(" + stream + ")";
                    case ProtoBuiltin.SInt64:
                        return "ProtocolParser.ReadSInt64(" + stream + ")";
                    case ProtoBuiltin.Fixed32:
                        return "br.ReadUInt32()";
                    case ProtoBuiltin.Fixed64:
                        return "br.ReadUInt64()";
                    case ProtoBuiltin.SFixed32:
                        return "br.ReadInt32()";
                    case ProtoBuiltin.SFixed64:
                        return "br.ReadInt64()";
                    case ProtoBuiltin.Bool:
                        return "ProtocolParser.ReadBool(" + stream + ")";
                    case ProtoBuiltin.String:
                        return "ProtocolParser.ReadString(" + stream + ")";
                    case ProtoBuiltin.Bytes:
                        return "ProtocolParser.ReadBytes(" + stream + ")";
                    default:
                        throw new ProtoFormatException("unknown build in: " + f.ProtoType.ProtoName);
                }   

            }

            throw new NotImplementedException();
        }

        #endregion
        
        
        
        #region Writer
        
        /// <summary>
        /// Generates code for writing one field
        /// </summary>
        public static void GenerateFieldWriter(ProtoMessage m, Field f, CodeWriter cw)
        {
            if (f.Rule == FieldRule.Repeated)
            {
                if (f.OptionPacked == true)
                {
                    cw.IfBracket("instance." + f.Name + " != null");
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + Wire.LengthDelimited + "));");
                    cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream()");
                    if (f.ProtoType is ProtoBuiltin)
                    {
                        switch (f.ProtoType.ProtoName)
                        {
                            case ProtoBuiltin.Double:
                            case ProtoBuiltin.Float:
                            case ProtoBuiltin.Fixed32:
                            case ProtoBuiltin.Fixed64:
                            case ProtoBuiltin.SFixed32:
                            case ProtoBuiltin.SFixed64:
                                cw.WriteLine("BinaryWriter bw" + f.ID + " = new BinaryWriter(ms" + f.ID + ");");
                                break;
                        }
                    }
                    cw.ForeachBracket("var i" + f.ID + " in instance." + f.Name);
                    cw.WriteLine(GenerateFieldTypeWriter(f, "ms" + f.ID, "bw" + f.ID, "i" + f.ID));
                    cw.EndBracket();
                    cw.WriteLine("ProtocolParser.WriteBytes(stream, ms" + f.ID + ".ToArray());");
                    cw.EndBracket();
                    cw.EndBracket();
                } else
                {
                    cw.IfBracket("instance." + f.Name + " != null");
                    cw.ForeachBracket("var i" + f.ID + " in instance." + f.Name);
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.ProtoType.WireType + "));");
                    cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "i" + f.ID));
                    cw.EndBracket();
                    cw.EndBracket();
                }
                return;
            } else if (f.Rule == FieldRule.Optional)
            {           
                if (f.ProtoType is ProtoMessage || 
                    f.ProtoType.ProtoName == ProtoBuiltin.String ||
                    f.ProtoType.ProtoName == ProtoBuiltin.Bytes)
                {
                    if (f.ProtoType.Nullable) //Struct always exist, not optional
                        cw.IfBracket("instance." + f.Name + " != null");
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.ProtoType.WireType + "));");
                    cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                    if (f.ProtoType.Nullable) //Struct always exist, not optional
                        cw.EndBracket();
                    return;
                }
                if (f.ProtoType is ProtoEnum)
                {
                    cw.IfBracket("instance." + f.Name + " != " + f.ProtoType.CsType + "." + f.OptionDefault);
                    cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.ProtoType.WireType + "));");
                    cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                    cw.EndBracket();
                    return;
                }
                cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.ProtoType.WireType + "));");
                cw.WriteLine(GenerateFieldTypeWriter(f, "stream", "bw", "instance." + f.Name));
                return;
            } else if (f.Rule == FieldRule.Required)
            {   
                if (f.ProtoType is ProtoMessage && f.ProtoType.OptionType != "struct" || 
                    f.ProtoType.ProtoName == ProtoBuiltin.String ||
                    f.ProtoType.ProtoName == ProtoBuiltin.Bytes)
                {
                    cw.WriteLine("if (instance." + f.Name + " == null)");
                    cw.WriteIndent("throw new ArgumentNullException(\"" + f.Name + "\", \"Required by proto specification.\");");
                }
                cw.WriteLine("ProtocolParser.WriteKey(stream, new ProtocolBuffers.Key(" + f.ID + ", Wire." + f.ProtoType.WireType + "));");
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

            if (f.ProtoType is ProtoEnum)
                return "ProtocolParser.WriteUInt32(" + stream + ",(uint)" + instance + ");";

            if (f.ProtoType is ProtoMessage)
            {
                ProtoMessage pm = f.ProtoType as ProtoMessage;
                CodeWriter cw = new CodeWriter();
                cw.Using("MemoryStream ms" + f.ID + " = new MemoryStream()");
                cw.WriteLine(pm.FullSerializerType + ".Serialize(ms" + f.ID + ", " + instance + ");");
                cw.WriteLine("ProtocolParser.WriteBytes(" + stream + ", ms" + f.ID + ".ToArray());");
                cw.EndBracket();
                return cw.Code;
            }

            switch (f.ProtoType.ProtoName)
            {
                case ProtoBuiltin.Double:
                case ProtoBuiltin.Float:
                case ProtoBuiltin.Fixed32:
                case ProtoBuiltin.Fixed64:
                case ProtoBuiltin.SFixed32:
                case ProtoBuiltin.SFixed64:
                    return binaryWriter + ".Write(" + instance + ");";
                case ProtoBuiltin.Int32:
                    return "ProtocolParser.WriteUInt32(" + stream + ",(uint)" + instance + ");";
                case ProtoBuiltin.Int64:
                    return "ProtocolParser.WriteUInt64(" + stream + ",(ulong)" + instance + ");";
                case ProtoBuiltin.UInt32:
                    return "ProtocolParser.WriteUInt32(" + stream + ", " + instance + ");";
                case ProtoBuiltin.UInt64:
                    return "ProtocolParser.WriteUInt64(" + stream + ", " + instance + ");";
                case ProtoBuiltin.SInt32:
                    return "ProtocolParser.WriteSInt32(" + stream + ", " + instance + ");";
                case ProtoBuiltin.SInt64:
                    return "ProtocolParser.WriteSInt64(" + stream + ", " + instance + ");";
                case ProtoBuiltin.Bool:
                    return "ProtocolParser.WriteBool(" + stream + ", " + instance + ");";
                case ProtoBuiltin.String:
                    return "ProtocolParser.WriteString(" + stream + ", " + instance + ");";
                case ProtoBuiltin.Bytes:
                    return "ProtocolParser.WriteBytes(" + stream + ", " + instance + ");";
            }

            throw new NotImplementedException();
        }
        
        #endregion
    }
}

