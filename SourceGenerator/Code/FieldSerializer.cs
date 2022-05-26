namespace SilentOrbit.ProtocolBuffers.Code;

class FieldSerializer
{
    readonly CodeWriter cw;

    public FieldSerializer(CodeWriter cw)
    {
        this.cw = cw;
    }

    #region Reader

    /// <summary>
    /// Return true for normal code and false if generated thrown exception.
    /// In the latter case a break is not needed to be generated afterwards.
    /// </summary>
    public bool FieldReader(Field f)
    {
        if (f.Rule == FieldRule.Repeated)
        {
            //Make sure we are not reading a list of interfaces
            if (f.ProtoType.OptionType == "interface")
            {
                cw.WriteLine("throw new NotSupportedException(\"Can't deserialize a list of interfaces\");");
                return false;
            }

            if (f.OptionPacked)
            {
                cw.Comment("repeated packed");
                cw.WriteLine("long end" + f.ID + " = " + "ReadUInt32(stream);");
                cw.WriteLine("end" + f.ID + " += stream.Position;");
                cw.WhileBracket("stream.Position < end" + f.ID);
                cw.WriteLine("instance." + f.CsName + ".Add(" + FieldReaderType(f, "stream", "br", null) + ");");
                cw.EndBracket();

                cw.WriteLine("if (stream.Position != end" + f.ID + ")");
                cw.WriteIndent("throw new ProtocolBufferException(\"Read too many bytes in packed data\");");
            }
            else
            {
                cw.Comment("repeated");
                cw.WriteLine("instance." + f.CsName + ".Add(" + FieldReaderType(f, "stream", "br", null) + ");");
            }
        }
        else
        {
            if (f.OptionReadOnly)
            {
                //The only "readonly" fields we can modify
                //We could possibly support bytes primitive too but it would require the incoming length to match the wire length
                if (f.ProtoType is ProtoMessage)
                {
                    cw.WriteLine(FieldReaderType(f, "stream", "br", "instance." + f.CsName) + ";");
                    return true;
                }
                cw.WriteLine("throw new InvalidOperationException(\"Can't deserialize into a readonly primitive field\");");
                return false;
            }

            if (f.ProtoType is ProtoMessage)
            {
                if (f.ProtoType.OptionType == "struct")
                {
                    cw.WriteLine(FieldReaderType(f, "stream", "br", "ref instance." + f.CsName) + ";");

                    return true;
                }

                cw.WriteLine("if (instance." + f.CsName + " == null)");
                if (f.ProtoType.OptionType == "interface")
                {
                    cw.WriteIndent("throw new InvalidOperationException(\"Can't deserialize into a interfaces null pointer\");");
                }
                else
                {
                    cw.WriteIndent("instance." + f.CsName + " = " + FieldReaderType(f, "stream", "br", null) + ";");
                }

                cw.WriteLine("else");
                cw.WriteIndent(FieldReaderType(f, "stream", "br", "instance." + f.CsName) + ";");
                return true;
            }

            cw.WriteLine("instance." + f.CsName + " = " + FieldReaderType(f, "stream", "br", "instance." + f.CsName) + ";");
        }
        return true;
    }

    /// <summary>
    /// Read a primitive from the stream
    /// </summary>
    string FieldReaderType(Field f, string stream, string binaryReader, string instance)
    {
        if (f.OptionCodeType != null)
        {
            switch (f.OptionCodeType)
            {
                case "DateTimeUTC":
                    switch (f.ProtoType.ProtoName)
                    {
                        case ProtoBuiltin.UInt64:
                        case ProtoBuiltin.Int64:
                        case ProtoBuiltin.Fixed64:
                        case ProtoBuiltin.SFixed64:
                            return "new DateTime((long)" + FieldReaderPrimitive(f, stream, binaryReader, instance) + ", DateTimeKind.Utc)";
                    }
                    throw new ProtoFormatException("Local feature, DateTime, must be stored in a 64 bit field", f.Source);

                case "DateTimeLocal":
                    switch (f.ProtoType.ProtoName)
                    {
                        case ProtoBuiltin.UInt64:
                        case ProtoBuiltin.Int64:
                        case ProtoBuiltin.Fixed64:
                        case ProtoBuiltin.SFixed64:
                            return "new DateTime((long)" + FieldReaderPrimitive(f, stream, binaryReader, instance) + ")";
                    }
                    throw new ProtoFormatException("Local feature, DateTime, must be stored in a 64 bit field", f.Source);

                case "TimeSpan":
                    switch (f.ProtoType.ProtoName)
                    {
                        case ProtoBuiltin.UInt64:
                        case ProtoBuiltin.Int64:
                        case ProtoBuiltin.Fixed64:
                        case ProtoBuiltin.SFixed64:
                            return "new TimeSpan((long)" + FieldReaderPrimitive(f, stream, binaryReader, instance) + ")";
                    }
                    throw new ProtoFormatException("Local feature, TimeSpan, must be stored in a 64 bit field", f.Source);

                default:
                    //Assume enum
                    return "(" + f.OptionCodeType + ")" + FieldReaderPrimitive(f, stream, binaryReader, instance);
            }
        }

        return FieldReaderPrimitive(f, stream, binaryReader, instance);
    }

    static string FieldReaderPrimitive(Field f, string stream, string binaryReader, string instance)
    {
        if (f.ProtoType is ProtoMessage m)
        {
            if (f.Rule == FieldRule.Repeated || instance == null)
            {
                return m.FullSerializerType + ".DeserializeLengthDelimited(" + stream + ")";
            }
            else
            {
                return m.FullSerializerType + ".DeserializeLengthDelimited(" + stream + ", " + instance + ")";
            }
        }

        if (f.ProtoType is ProtoEnum)
        {
            return "(" + f.ProtoType.FullCsType + ")" + "ReadUInt64(" + stream + ")";
        }

        if (f.ProtoType is ProtoBuiltin)
        {
            switch (f.ProtoType.ProtoName)
            {
                case ProtoBuiltin.Double:
                    return binaryReader + ".ReadDouble()";
                case ProtoBuiltin.Float:
                    return binaryReader + ".ReadSingle()";
                case ProtoBuiltin.Int32: //Wire format is 64 bit varint
                    return "(int)" + "ReadUInt64(" + stream + ")";
                case ProtoBuiltin.Int64:
                    return "(long)" + "ReadUInt64(" + stream + ")";
                case ProtoBuiltin.UInt32:
                    return "ReadUInt32(" + stream + ")";
                case ProtoBuiltin.UInt64:
                    return "ReadUInt64(" + stream + ")";
                case ProtoBuiltin.SInt32:
                    return "ReadZInt32(" + stream + ")";
                case ProtoBuiltin.SInt64:
                    return "ReadZInt64(" + stream + ")";
                case ProtoBuiltin.Fixed32:
                    return binaryReader + ".ReadUInt32()";
                case ProtoBuiltin.Fixed64:
                    return binaryReader + ".ReadUInt64()";
                case ProtoBuiltin.SFixed32:
                    return binaryReader + ".ReadInt32()";
                case ProtoBuiltin.SFixed64:
                    return binaryReader + ".ReadInt64()";
                case ProtoBuiltin.Bool:
                    return "ReadBool(" + stream + ")";
                case ProtoBuiltin.String:
                    return "ReadString(" + stream + ")";
                case ProtoBuiltin.Bytes:
                    return "ReadBytes(" + stream + ")";
                default:
                    throw new ProtoFormatException("unknown build in: " + f.ProtoType.ProtoName, f.Source);
            }
        }

        throw new NotImplementedException();
    }

    #endregion Reader

    #region Writer

    static void KeyWriter(string stream, int id, Wire wire, CodeWriter cw)
    {
        var n = (uint)id << 3 | (uint)wire;
        cw.Comment("Key for field: " + id + ", " + wire);
        //cw.WriteLine("ProtocolParser.WriteUInt32(" + stream + ", " + n + ");");
        VarintWriter(stream, n, cw);
    }

    /// <summary>
    /// Generates writer for a varint value known at compile time
    /// </summary>
    static void VarintWriter(string stream, uint value, CodeWriter cw)
    {
        while (true)
        {
            var b = (byte)(value & 0x7F);
            value >>= 7;
            if (value == 0)
            {
                cw.WriteLine(stream + ".WriteByte(" + b + ");");
                break;
            }

            //Write part of value
            b |= 0x80;
            cw.WriteLine(stream + ".WriteByte(" + b + ");");
        }
    }

    /// <summary>
    /// Generates inline writer of a length delimited byte array
    /// </summary>
    static void BytesWriter(Field f, string stream, CodeWriter cw)
    {
        cw.Comment("Length delimited byte array");

        //Original
        //cw.WriteLine("ProtocolParser.WriteBytes(" + stream + ", " + memoryStream + ".ToArray());");

        //Much slower than original
        /*
        cw.WriteLine("ProtocolParser.WriteUInt32(" + stream + ", (uint)" + memoryStream + ".Length);");
        cw.WriteLine(memoryStream + ".Seek(0, System.IO.SeekOrigin.Begin);");
        cw.WriteLine(memoryStream + ".CopyTo(" + stream + ");");
        */

        //Same speed as original
        /*
        cw.WriteLine("ProtocolParser.WriteUInt32(" + stream + ", (uint)" + memoryStream + ".Length);");
        cw.WriteLine(stream + ".Write(" + memoryStream + ".ToArray(), 0, (int)" + memoryStream + ".Length);");
        */

        //10% faster than original using GetBuffer rather than ToArray
        cw.WriteLine("uint length" + f.ID + " = (uint)msField.Length;");
        cw.WriteLine("WriteUInt32(" + stream + ", length" + f.ID + ");");
        cw.WriteLine("msField.WriteTo(" + stream + ");");
    }

    /// <summary>
    /// Generates code for writing one field
    /// </summary>
    public void FieldWriter(Field f)
    {
        if (f.Rule == FieldRule.Repeated)
        {
            if (f.OptionPacked)
            {
                //Repeated packed
                cw.IfBracket("instance." + f.CsName + " != null");

                KeyWriter("stream", f.ID, Wire.LengthDelimited, cw);
                if (f.ProtoType.WireSize < 0)
                {
                    //Un-optimized, unknown size
                    cw.WriteLine("msField.SetLength(0);");
                    if (f.IsUsingBinaryWriter)
                    {
                        cw.WriteLine("BinaryWriter bw" + f.ID + " = new BinaryWriter(ms" + f.ID + ");");
                    }

                    cw.ForeachBracket("var i" + f.ID + " in instance." + f.CsName);
                    FieldWriterType(f, "msField", "bw" + f.ID, "i" + f.ID);
                    cw.EndBracket();

                    BytesWriter(f, "stream", cw);
                }
                else
                {
                    //Optimized with known size
                    //No memorystream buffering, write size first at once

                    //For constant size messages we can skip serializing to the MemoryStream
                    cw.WriteLine("WriteUInt32(stream, " + f.ProtoType.WireSize + "u * (uint)instance." + f.CsName + ".Count);");

                    cw.ForeachBracket("var i" + f.ID + " in instance." + f.CsName);
                    FieldWriterType(f, "stream", "bw", "i" + f.ID);
                    cw.EndBracket();
                }
                cw.EndBracket();
            }
            else
            {
                //Repeated not packet
                cw.IfBracket("instance." + f.CsName + " != null");
                cw.ForeachBracket("var i" + f.ID + " in instance." + f.CsName);
                KeyWriter("stream", f.ID, f.ProtoType.WireType, cw);
                FieldWriterType(f, "stream", "bw", "i" + f.ID);
                cw.EndBracket();
                cw.EndBracket();
            }
            return;
        }
        else if (f.Rule == FieldRule.Optional)
        {
            var skip = f.OptionDefault != null;

            if (f.ProtoType is ProtoMessage ||
                f.ProtoType.ProtoName == ProtoBuiltin.String ||
                f.ProtoType.ProtoName == ProtoBuiltin.Bytes)
            {
                if (f.ProtoType.Nullable) //Struct always exist, not optional
                {
                    cw.IfBracket("instance." + f.CsName + " != null");
                }

                KeyWriter("stream", f.ID, f.ProtoType.WireType, cw);
                FieldWriterType(f, "stream", "bw", "instance." + f.CsName);
                if (f.ProtoType.Nullable) //Struct always exist, not optional
                {
                    cw.EndBracket();
                }

                return;
            }
            if (f.ProtoType is ProtoEnum)
            {
                if (skip)
                {
                    cw.IfBracket("instance." + f.CsName + " != " + f.FormatDefaultForTypeAssignment());
                }

                KeyWriter("stream", f.ID, f.ProtoType.WireType, cw);
                FieldWriterType(f, "stream", "bw", "instance." + f.CsName);
                if (skip)
                {
                    cw.EndBracket();
                }

                return;
            }
            if (skip) //Skip writing value if default
            {
                cw.IfBracket("instance." + f.CsName + " != " + f.FormatDefaultForTypeAssignment());
            }

            KeyWriter("stream", f.ID, f.ProtoType.WireType, cw);
            FieldWriterType(f, "stream", "bw", "instance." + f.CsName);
            if (skip)
            {
                cw.EndBracket();
            }

            return;
        }
        else if (f.Rule == FieldRule.Required)
        {
            if (f.ProtoType is ProtoMessage && f.ProtoType.OptionType != "struct" ||
                f.ProtoType.ProtoName == ProtoBuiltin.String ||
                f.ProtoType.ProtoName == ProtoBuiltin.Bytes)
            {
                cw.WriteLine("if (instance." + f.CsName + " == null)");
                cw.WriteIndent("throw new ProtocolBufferException(\"" + f.CsName + " is required by the proto specification.\");");
            }
            KeyWriter("stream", f.ID, f.ProtoType.WireType, cw);
            FieldWriterType(f, "stream", "bw", "instance." + f.CsName);
            return;
        }
        throw new NotImplementedException("Unknown rule: " + f.Rule);
    }

    void FieldWriterType(Field f, string stream, string binaryWriter, string instance)
    {
        if (f.OptionCodeType != null)
        {
            switch (f.OptionCodeType)
            {
                case "DateTimeUTC":
                    cw.WriteLine(FieldWriterPrimitive(f, stream, binaryWriter, "(" + instance + ".Kind == DateTimeKind.Utc ? " + instance + " : " + instance + ".ToUniversalTime()).Ticks"));
                    return;
                case "DateTimeLocal":
                    cw.WriteLine(FieldWriterPrimitive(f, stream, binaryWriter, instance + ".Ticks"));
                    return;
                case "TimeSpan":
                    cw.WriteLine(FieldWriterPrimitive(f, stream, binaryWriter, instance + ".Ticks"));
                    return;
                default: //enum
                    break;
            }
        }
        cw.WriteLine(FieldWriterPrimitive(f, stream, binaryWriter, instance));
        return;
    }

    static string FieldWriterPrimitive(Field f, string stream, string binaryWriter, string instance)
    {
        if (f.ProtoType is ProtoEnum)
        {
            return "WriteUInt64(" + stream + ",(ulong)" + instance + ");";
        }

        if (f.ProtoType is ProtoMessage)
        {
            var cw = new CodeWriter();
            cw.WriteLine("msField.SetLength(0);");

            var pm = (ProtoMessage)f.ProtoType;
            cw.WriteLine(pm.FullSerializerType + ".Serialize(msField, " + instance + ");");
            BytesWriter(f, stream, cw);
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
            case ProtoBuiltin.Int32: //Serialized as 64 bit varint
                return "WriteUInt64(" + stream + ",(ulong)" + instance + ");";
            case ProtoBuiltin.Int64:
                return "WriteUInt64(" + stream + ",(ulong)" + instance + ");";
            case ProtoBuiltin.UInt32:
                return "WriteUInt32(" + stream + ", " + instance + ");";
            case ProtoBuiltin.UInt64:
                return "WriteUInt64(" + stream + ", " + instance + ");";
            case ProtoBuiltin.SInt32:
                return "WriteZInt32(" + stream + ", " + instance + ");";
            case ProtoBuiltin.SInt64:
                return "WriteZInt64(" + stream + ", " + instance + ");";
            case ProtoBuiltin.Bool:
                return "WriteBool(" + stream + ", " + instance + ");";
            case ProtoBuiltin.String:
                return "WriteBytes(" + stream + ", Encoding.UTF8.GetBytes(" + instance + "));";
            case ProtoBuiltin.Bytes:
                return "WriteBytes(" + stream + ", " + instance + ");";
        }

        throw new NotImplementedException();
    }

    #endregion Writer
}
