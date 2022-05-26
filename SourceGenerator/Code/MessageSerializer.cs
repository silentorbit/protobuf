namespace SilentOrbit.ProtocolBuffers.Code;

class MessageSerializer
{
    readonly CodeWriter cw;
    readonly FieldSerializer fieldSerializer;

    public MessageSerializer(CodeWriter cw)
    {
        this.cw = cw;
        fieldSerializer = new FieldSerializer(cw);
    }

    public void GenerateClassSerializer(ProtoMessage m)
    {
        if (m.OptionExternal || m.OptionType == "interface")
        {
            //Don't make partial class of external classes or interfaces
            //Make separate static class for them
            cw.Bracket(m.OptionAccess + " static class " + m.SerializerType);
        }
        else
        {
            cw.Bracket(m.OptionAccess + " partial " + m.OptionType + " " + m.SerializerType);
        }

        GenerateReader(m);

        GenerateWriter(m);
        foreach (var sub in m.Messages.Values)
        {
            cw.WriteLine();
            GenerateClassSerializer(sub);
        }
        cw.EndBracket();
        cw.WriteLine();
        return;
    }

    void GenerateReader(ProtoMessage m)
    {
        #region Helper Deserialize Methods
        var refstr = m.OptionType == "struct" ? "ref " : "";
        if (m.OptionType != "interface")
        {
            cw.Summary("Helper: create a new instance to deserializing into");
            cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(Stream stream)");
            cw.WriteLine("var instance = new " + m.CsType + "();");
            cw.WriteLine("Deserialize(stream, " + refstr + "instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();

            cw.Summary("Helper: create a new instance to deserializing into");
            cw.Bracket(m.OptionAccess + " static " + m.CsType + " DeserializeLengthDelimited(Stream stream)");
            cw.WriteLine("var instance = new " + m.CsType + "();");
            cw.WriteLine("DeserializeLengthDelimited(stream, " + refstr + "instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();

            cw.Summary("Helper: create a new instance to deserializing into");
            cw.Bracket(m.OptionAccess + " static " + m.CsType + " DeserializeLength(Stream stream, int length)");
            cw.WriteLine("var instance = new " + m.CsType + "();");
            cw.WriteLine("DeserializeLength(stream, length, " + refstr + "instance);");
            cw.WriteLine("return instance;");
            cw.EndBracketSpace();

            cw.Summary("Helper: put the buffer into a MemoryStream and create a new instance to deserializing into");
            cw.Bracket(m.OptionAccess + " static " + m.CsType + " Deserialize(byte[] buffer)");
            cw.WriteLine("var instance = new " + m.CsType + "();");
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
        #endregion Helper Deserialize Methods

        var methods = new string[]
        {
                "Deserialize", //Default old one
                "DeserializeLengthDelimited", //Start by reading length prefix and stay within that limit
                "DeserializeLength", //Read at most length bytes given by argument
        };

        //Main Deserialize
        foreach (var method in methods)
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
            {
                throw new NotImplementedException();
            }

            if (m.IsUsingBinaryWriter)
            {
                cw.WriteLine("var br = new BinaryReader(stream);");
            }

            //Prepare List<> and default values
            foreach (var f in m.Fields.Values)
            {
                if (f.OptionDeprecated)
                {
                    cw.WritePragma("warning disable 612");
                }

                if (f.Rule == FieldRule.Repeated)
                {
                    if (f.OptionReadOnly == false)
                    {
                        //Initialize lists of the custom DateTime or TimeSpan type.
                        var csType = f.ProtoType.FullCsType;
                        if (f.OptionCodeType != null)
                        {
                            switch (f.OptionCodeType)
                            {
                                case "DateTimeUTC":
                                case "DateTimeLocal":
                                    csType = "DateTime";
                                    break;
                                default:
                                    csType = f.OptionCodeType;
                                    break;
                            }
                        }

                        cw.WriteLine("if (instance." + f.CsName + " == null)");
                        cw.WriteIndent("instance." + f.CsName + " = new List<" + csType + ">();");
                    }
                }
                else if (f.OptionDefault != null)
                {
                    cw.WriteLine("instance." + f.CsName + " = " + f.FormatDefaultForTypeAssignment() + ";");
                }
                else if (f.Rule == FieldRule.Optional)
                {
                    if (f.ProtoType is ProtoEnum pe)
                    {
                        //the default value is the first value listed in the enum's type definition
                        foreach (var kvp in pe.Enums)
                        {
                            cw.WriteLine("instance." + f.CsName + " = " + pe.FullCsType + "." + kvp.Name + ";");
                            break;
                        }
                    }
                }

                if (f.OptionDeprecated)
                {
                    cw.WritePragma("warning restore 612");
                }
            }

            if (method == "DeserializeLengthDelimited")
            {
                //Important to read stream position after we have read the length field
                cw.WriteLine("long limit = " + "ReadUInt32(stream);");
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
                cw.WriteIndent("throw new ProtocolBufferException(\"Read past max limit\");");
                cw.EndBracket();
            }

            cw.WriteLine("int keyByte = stream.ReadByte();");
            cw.WriteLine("if (keyByte == -1)");
            if (method == "Deserialize")
            {
                cw.WriteIndent("break;");
            }
            else
            {
                cw.WriteIndent("throw new System.IO.EndOfStreamException();");
            }

            //Determine if we need the lowID optimization
            var hasLowID = false;
            foreach (var f in m.Fields.Values)
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
                foreach (var f in m.Fields.Values)
                {
                    if (f.ID >= 16)
                    {
                        continue;
                    }

                    if (f.OptionDeprecated)
                    {
                        cw.WritePragma("warning disable 612");
                    }

                    cw.Dedent();
                    cw.Comment("Field " + f.ID + " " + f.WireType);
                    cw.Indent();
                    cw.Case(f.ID << 3 | (int)f.WireType);
                    if (fieldSerializer.FieldReader(f))
                    {
                        cw.WriteLine("continue;");
                    }

                    if (f.OptionDeprecated)
                    {
                        cw.WritePragma("warning restore 612");
                    }
                }
                cw.SwitchEnd();
                cw.WriteLine();
            }
            cw.WriteLine("var key = " + "ReadKey((byte)keyByte, stream);");

            cw.WriteLine();

            cw.Comment("Reading field ID > 16 and unknown field ID/wire type combinations");
            cw.Switch("key.Field");
            cw.Case(0);
            cw.WriteLine("throw new ProtocolBufferException(\"Invalid field id: 0, something went wrong in the stream\");");
            foreach (var f in m.Fields.Values)
            {
                if (f.ID < 16)
                {
                    continue;
                }

                cw.Case(f.ID);
                //Makes sure we got the right wire type
                cw.WriteLine("if(key.WireType != Wire." + f.WireType + ")");
                cw.WriteIndent("break;"); //This can be changed to throw an exception for unknown formats.

                if (f.OptionDeprecated)
                {
                    cw.WritePragma("warning disable 612");
                }

                if (fieldSerializer.FieldReader(f))
                {
                    cw.WriteLine("continue;");
                }

                if (f.OptionDeprecated)
                {
                    cw.WritePragma("warning restore 612");
                }
            }
            cw.CaseDefault();
            if (m.OptionPreserveUnknown)
            {
                cw.WriteLine("if (instance.PreservedFields == null)");
                cw.WriteIndent("instance.PreservedFields = new List<KeyValue>();");
                cw.WriteLine("instance.PreservedFields.Add(new KeyValue(key, " + "ReadValueBytes(stream, key)));");
            }
            else
            {
                cw.WriteLine("SkipKey(stream, key);");
            }
            cw.WriteLine("break;");
            cw.SwitchEnd();
            cw.EndBracket();
            cw.WriteLine();

            if (m.OptionTriggers)
            {
                cw.WriteLine("instance.AfterDeserialize();");
            }

            cw.WriteLine("return instance;");
            cw.EndBracket();
            cw.WriteLine();
        }

        return;
    }

    /// <summary>
    /// Generates code for writing a class/message
    /// </summary>
    void GenerateWriter(ProtoMessage m)
    {
        cw.Summary("Serialize the instance into the stream");
        cw.Bracket(m.OptionAccess + " static void Serialize(Stream stream, " + m.CsType + " instance)");
        if (m.OptionTriggers)
        {
            cw.WriteLine("instance.BeforeSerialize();");
            cw.WriteLine();
        }
        if (m.IsUsingBinaryWriter)
        {
            cw.WriteLine("var bw = new BinaryWriter(stream);");
        }

        //Shared memorystream for all fields
        cw.Using("var msField = new MemoryStream()");

        foreach (var f in m.Fields.Values)
        {
            if (f.OptionDeprecated)
            {
                cw.WritePragma("warning disable 612");
            }

            fieldSerializer.FieldWriter(f);

            if (f.OptionDeprecated)
            {
                cw.WritePragma("warning restore 612");
            }
        }

        cw.EndBracket();

        if (m.OptionPreserveUnknown)
        {
            cw.IfBracket("instance.PreservedFields != null");
            cw.ForeachBracket("var kv in instance.PreservedFields");
            cw.WriteLine("WriteKey(stream, kv.Key);");
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
        cw.WriteLine("WriteUInt32(stream, (uint)data.Length);");
        cw.WriteLine("stream.Write(data, 0, data.Length);");
        cw.EndBracket();
    }
}
