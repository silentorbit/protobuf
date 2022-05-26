namespace SilentOrbit.ProtocolBuffers.Parsers;

class ProtoPrepare
{
    /// <summary>
    /// Convert message/class and field/propery names to CamelCase
    /// </summary>
    const bool ConvertToCamelCase = true;

    public void Prepare(ProtoCollection file)
    {
        foreach (var m in file.Messages.Values)
        {
            if (m.OptionNamespace == null)
            {
                m.OptionNamespace = GetCamelCase(m.Package);
            }

            PrepareMessage(m);
        }

        foreach (var e in file.Enums.Values)
        {
            if (e.OptionNamespace == null)
            {
                e.OptionNamespace = GetCamelCase(e.Package);
            }

            e.CsType = GetCamelCase(e.ProtoName);
        }
    }

    void PrepareMessage(ProtoMessage m)
    {
        //Name of message and enums
        m.CsType = GetCamelCase(m.ProtoName);
        foreach (var e in m.Enums.Values)
        {
            e.CsType = GetCamelCase(e.ProtoName);
        }

        foreach (var sub in m.Messages.Values)
        {
            PrepareMessage(sub);
        }

        //Prepare fields
        foreach (var f in m.Fields.Values)
        {
            PrepareProtoType(m, f);

            DetectNameClash(m, f);

            if (f.OptionDefault != null)
            {
                if (f.ProtoType is ProtoBuiltin && ((ProtoBuiltin)f.ProtoType).ProtoName == "bytes")
                {
                    throw new NotImplementedException();
                }

                if (f.ProtoType is ProtoMessage)
                {
                    throw new ProtoFormatException("Message can't have a default", f.Source);
                }
            }
        }
    }


    /// <summary>
    /// Detect field which have the same name as a submessage in the same message.
    /// </summary>
    /// <param name="m">Parent message</param>
    /// <param name="f">Field to check</param>
    void DetectNameClash(ProtoMessage m, Field f)
    {
        bool nameclash = m.CsType == f.CsName;
        foreach (var tm in m.Messages.Values)
        {
            if (tm.CsType == f.CsName)
            {
                nameclash = true;
            }
        }

        foreach (var te in m.Enums.Values)
        {
            if (te.CsType == f.CsName)
            {
                nameclash = true;
            }
        }

        foreach (var tf in m.Fields.Values)
        {
            if (tf == f)
            {
                continue;
            }

            if (tf.CsName == f.CsName)
            {
                nameclash = true;
            }
        }
        if (nameclash == false)
        {
            return;
        }

        //Name clash
        if (ConvertToCamelCase)
        {
            f.CsName += "Field";
        }
        else
        {
#pragma warning disable CS0162 // Unreachable code detected
            f.CsName += "_field";
#pragma warning restore CS0162 // Unreachable code detected
        }

        Console.Error.WriteLine("Warning: renamed field: " + m.FullCsType + "." + f.CsName);

        //Make sure our change did not result in another name collission
        DetectNameClash(m, f);
    }

    /// <summary>
    /// Prepare: ProtoType, WireType and CSType
    /// </summary>
    void PrepareProtoType(ProtoMessage m, Field f)
    {
        //Change property name to C# style, CamelCase.
        f.CsName = GetCSPropertyName(m, f.ProtoName);

        f.ProtoType = GetBuiltinProtoType(f.ProtoTypeName) ?? Search.GetProtoType(m, f.ProtoTypeName);

        if (f.ProtoType == null)
        {
#if DEBUG
            //this will still return null but we keep it here for debugging purposes
            f.ProtoType = Search.GetProtoType(m, f.ProtoTypeName);
#endif
            throw new ProtoFormatException("Field type \"" + f.ProtoTypeName + "\" not found for field " + f.ProtoName + " in message " + m.FullProtoName, f.Source);
        }

        if (f.OptionPacked)
        {
            if (f.ProtoType.WireType == Wire.LengthDelimited)
            {
                throw new ProtoFormatException("Length delimited types cannot be packed", f.Source);
            }
        }
    }

    /// <summary>
    /// Return the type given the name from a .proto file.
    /// Return Unknonw if it is a message or an enum.
    /// </summary>
    static ProtoBuiltin GetBuiltinProtoType(string type)
    {
        switch (type)
        {
            case "double":
                return new ProtoBuiltin(type, Wire.Fixed64, "double");
            case "float":
                return new ProtoBuiltin(type, Wire.Fixed32, "float");
            case "int32":
                return new ProtoBuiltin(type, Wire.Varint, "int");
            case "int64":
                return new ProtoBuiltin(type, Wire.Varint, "long");
            case "uint32":
                return new ProtoBuiltin(type, Wire.Varint, "uint");
            case "uint64":
                return new ProtoBuiltin(type, Wire.Varint, "ulong");
            case "sint32":
                return new ProtoBuiltin(type, Wire.Varint, "int");
            case "sint64":
                return new ProtoBuiltin(type, Wire.Varint, "long");
            case "fixed32":
                return new ProtoBuiltin(type, Wire.Fixed32, "uint");
            case "fixed64":
                return new ProtoBuiltin(type, Wire.Fixed64, "ulong");
            case "sfixed32":
                return new ProtoBuiltin(type, Wire.Fixed32, "int");
            case "sfixed64":
                return new ProtoBuiltin(type, Wire.Fixed64, "long");
            case "bool":
                return new ProtoBuiltin(type, Wire.Varint, "bool");
            case "string":
                return new ProtoBuiltin(type, Wire.LengthDelimited, "string");
            case "bytes":
                return new ProtoBuiltin(type, Wire.LengthDelimited, "byte[]");
            default:
                return null;
        }
    }

    /// <summary>
    /// Gets the C# CamelCase version of a given name.
    /// Name collisions with enums are avoided.
    /// </summary>
    string GetCSPropertyName(ProtoMessage m, string name)
    {
        var csname = GetCamelCase(name);

        foreach (var me in m.Enums.Values)
        {
            if (me.CsType == csname)
            {
                return name;
            }
        }

        return csname;
    }

    /// <summary>
    /// Gets the CamelCase version of a given name.
    /// </summary>
    string GetCamelCase(string name)
    {
        if (ConvertToCamelCase == false)
        {
#pragma warning disable CS0162 // Unreachable code detected
            return name;
#pragma warning restore CS0162 // Unreachable code detected
        }

        return Name.ToCamelCase(name);
    }
}
