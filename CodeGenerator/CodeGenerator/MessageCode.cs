using System;

namespace ProtocolBuffers
{
    static class MessageCode
    {
        public static void GenerateClass(ProtoMessage m, CodeWriter cw)
        {
            //Do not generate class code for external classes
            if (m.OptionExternal || m.OptionImported)
            {
                cw.Comment("Written elsewhere");
                cw.Comment(m.OptionAccess + " " + m.OptionType + " " + m.CsType + " {}");
                return;
            }

            //Default class
            cw.Summary(m.Comments);
            cw.Bracket(m.OptionAccess + " partial " + m.OptionType + " " + m.CsType);

            GenerateEnums(m, cw);

            GenerateProperties(m, cw);

            if (m.OptionPreserveUnknown)
            {
                cw.Summary("Values for unknown fields.");
                cw.WriteLine("public List<ProtocolBuffers.KeyValue> PreservedFields;");
                cw.WriteLine();
            }
            
            if (m.OptionTriggers)
            {
                cw.Comment("protected virtual void BeforeSerialize() {}");
                cw.Comment("protected virtual void AfterDeserialize() {}");
                cw.WriteLine();
            }
            
            foreach (ProtoMessage sub in m.Messages)
            {
                GenerateClass(sub, cw);
                cw.WriteLine();
            }
            cw.EndBracket();
            return;
        }

        static void GenerateEnums(ProtoMessage m, CodeWriter cw)
        {
            foreach (ProtoEnum me in m.Enums)
            {
                cw.Bracket("public enum " + me.CsType);
                foreach (var epair in me.Enums)
                {
                    if (me.EnumsComments.ContainsKey(epair.Key))
                        cw.Summary(me.EnumsComments [epair.Key]);
                    cw.WriteLine(epair.Key + " = " + epair.Value + ",");
                }
                cw.EndBracket();
                cw.WriteLine();
            }
        }

        /// <summary>
        /// Generates the properties.
        /// </summary>
        /// <param name='template'>
        /// if true it will generate only properties that are not included by default, because of the [generate=false] option.
        /// </param>
        static void GenerateProperties(ProtoMessage m, CodeWriter cw)
        {
            foreach (Field f in m.Fields.Values)
            {
                if (f.OptionGenerate)
                {
                    if (f.Comments != null) 
                        cw.Summary(f.Comments);
                    cw.WriteLine(GenerateProperty(f));
                    cw.WriteLine();
                } else
                {
                    cw.WriteLine("//" + GenerateProperty(f) + " //Implemented by user elsewhere");
                }
            }
        }
        
        static string GenerateProperty(Field f)
        {
            string type = f.ProtoType.FullCsType;
            if(f.OptionCodeType != null)
                type = f.OptionCodeType;
            if (f.Rule == FieldRule.Repeated)
                type = "List<" + f.ProtoType.FullCsType + ">";

            if (f.OptionReadOnly)
                return f.OptionAccess + " readonly " + type + " " + f.Name + " = new " + type + "();";
            else if (f.ProtoType is ProtoMessage && f.ProtoType.OptionType == "struct")
                return f.OptionAccess + " " + type + " " + f.Name + ";";
            else
                return f.OptionAccess + " " + type + " " + f.Name + " { get; set; }";
        }
    }
}

