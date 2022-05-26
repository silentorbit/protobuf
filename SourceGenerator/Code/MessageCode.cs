namespace SilentOrbit.ProtocolBuffers.Code;

class MessageCode
{
    readonly CodeWriter cw;

    public MessageCode(CodeWriter cw)
    {
        this.cw = cw;
    }

    public void GenerateClass(ProtoMessage m)
    {
        //Default class
        cw.Summary(m.Comments);
        cw.Bracket(m.OptionAccess + " partial " + m.OptionType + " " + m.CsType);

        GenerateCtorForDefaults(m);

        GenerateEnums(m);

        GenerateProperties(m);

        //if(options.GenerateToString...
        // ...

        if (m.OptionPreserveUnknown)
        {
            cw.Summary("Values for unknown fields.");
            cw.WriteLine("public List<KeyValue> PreservedFields;");
            cw.WriteLine();
        }

        if (m.OptionTriggers)
        {
            cw.Comment("protected virtual void BeforeSerialize() {}");
            cw.Comment("protected virtual void AfterDeserialize() {}");
            cw.WriteLine();
        }

        foreach (var sub in m.Messages.Values)
        {
            GenerateClass(sub);
            cw.WriteLine();
        }
        cw.EndBracket();
        return;
    }

    void GenerateCtorForDefaults(ProtoMessage m)
    {
        // Collect all fields with default values.
        var fieldsWithDefaults = new List<Field>();
        foreach (var field in m.Fields.Values)
        {
            if (field.OptionDefault != null)
            {
                fieldsWithDefaults.Add(field);
            }
        }

        if (fieldsWithDefaults.Count > 0)
        {
            cw.Bracket("public " + m.CsType + "()");
            foreach (var field in fieldsWithDefaults)
            {
                var formattedValue = field.FormatDefaultForTypeAssignment();
                var line = string.Format("{0} = {1};", field.CsName, formattedValue);
                cw.WriteLine(line);
            }
            cw.EndBracket();
        }
    }

    void GenerateEnums(ProtoMessage m)
    {
        foreach (var me in m.Enums.Values)
        {
            GenerateEnum(me);
        }
    }

    public void GenerateEnum(ProtoEnum m)
    {
        if (m.OptionExternal)
        {
            cw.Comment("Written elsewhere");
            cw.Comment(m.Comments);
            cw.Comment(m.OptionAccess + " enum " + m.CsType);
            cw.Comment("{");
            foreach (var epair in m.Enums)
            {
                cw.Summary(epair.Comment);
                cw.Comment(cw.IndentPrefix + epair.Name + " = " + epair.Value + ",");
            }
            cw.Comment("}");
            return;
        }

        cw.Summary(m.Comments);
        if (m.OptionFlags)
        {
            cw.Attribute("global::System.FlagsAttribute");
        }

        cw.Bracket(m.OptionAccess + " enum " + m.CsType);
        foreach (var epair in m.Enums)
        {
            cw.Summary(epair.Comment);
            cw.WriteLine(epair.Name + " = " + epair.Value + ",");
        }
        cw.EndBracket();
        cw.WriteLine();
    }

    /// <summary>
    /// Generates the properties.
    /// </summary>
    void GenerateProperties(ProtoMessage m)
    {
        foreach (var f in m.Fields.Values)
        {
            if (f.Comments != null)
            {
                cw.Summary(f.Comments);
            }

            if (f.OptionExternal)
            {
                if (f.OptionDeprecated)
                {
                    cw.WriteLine("// [Obsolete]");
                }

                cw.WriteLine("//" + GenerateProperty(f) + " // Implemented by user elsewhere");
            }
            else
            {
                if (f.OptionDeprecated)
                {
                    cw.WriteLine("[Obsolete]");
                }

                cw.WriteLine(GenerateProperty(f));
            }
            cw.WriteLine();
        }

        //Wire format field ID
#if DEBUGx
            cw.Comment("ProtocolBuffers wire field id");
            foreach (Field f in m.Fields.Values)
            {
                cw.WriteLine("public const int " + f.CsName + "FieldID = " + f.ID + ";");
            }
#endif
    }

    string GenerateProperty(Field f)
    {
        var csType = f.ProtoType.CsType;
        if (f.ProtoType is ProtoBuiltin == false)
            if (f.ProtoType.CsNamespace != f.Parent.CsNamespace)
                csType = f.ProtoType.FullCsType;

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

        if (f.Rule == FieldRule.Repeated)
        {
            csType = "List<" + csType + ">";
        }

        if (f.OptionReadOnly)
        {
            return f.OptionAccess + " readonly " + csType + " " + f.CsName + " = new " + csType + "();";
        }
        else if (f.ProtoType is ProtoMessage && f.ProtoType.OptionType == "struct")
        {
            return f.OptionAccess + " " + csType + " " + f.CsName + ";";
        }
        else
        {
            return f.OptionAccess + " " + csType + " " + f.CsName + " { get; set; }";
        }
    }
}
