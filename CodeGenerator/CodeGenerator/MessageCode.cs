using System;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    static class MessageCode
    {
        public static void GenerateClass(ProtoMessage m, CodeWriter cw)
        {
            //Do not generate class code for external classes
            if (m.OptionExternal)
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
                cw.WriteLine("public List<global::SilentOrbit.ProtocolBuffers.KeyValue> PreservedFields;");
                cw.WriteLine();
            }

            if (m.OptionTriggers)
            {
                cw.Comment("protected virtual void BeforeSerialize() {}");
                cw.Comment("protected virtual void AfterDeserialize() {}");
                cw.WriteLine();
            }

            foreach (ProtoMessage sub in m.Messages.Values)
            {
                GenerateClass(sub, cw);
                cw.WriteLine();
            }
            cw.EndBracket();
            return;
        }

        static void GenerateEnums(ProtoMessage m, CodeWriter cw)
        {
            foreach (ProtoEnum me in m.Enums.Values)
            {
                GenerateEnum(me, cw);
            }
        }

        public static void GenerateEnum(ProtoEnum me, CodeWriter cw)
        {
            cw.Bracket("public enum " + me.CsType);
            foreach (var epair in me.Enums)
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
        /// <param name='template'>
        /// if true it will generate only properties that are not included by default, because of the [generate=false] option.
        /// </param>
        static void GenerateProperties(ProtoMessage m, CodeWriter cw)
        {
            foreach (Field f in m.Fields.Values)
            {
                if (f.OptionExternal)
                    cw.WriteLine("//" + GenerateProperty(f) + " // Implemented by user elsewhere");
                else
                {
                    if (f.Comments != null)
                        cw.Summary(f.Comments);
                    cw.WriteLine(GenerateProperty(f));
                    cw.WriteLine();
                }

            }

            // Implement ToString
            cw.Bracket("public override string ToString()");
            string returnStatement = "return \"\"";
            Dictionary<int, Field>.ValueCollection fields = m.Fields.Values;
            if (fields.Count > 0)
            {
                List<string> fieldElements = new List<string>();
                foreach (Field f in fields)
                {
                    string fieldHeaderCode = "\"" + f.CsName + ": \" + ";
                    string fieldToStringCode;
                    string fieldCommaCode = " + \", \"";

                    if (f.Rule == FieldRule.Optional && f.ProtoType.Nullable)
                    {
                        // Hide optional nullable fields: this makes logging cleaner for union types
                        fieldToStringCode = string.Format("({0} != null ? {1}{0}{2}:  \"\")", f.CsName, fieldHeaderCode, fieldCommaCode);
                    } 
                    else if (f.Rule == FieldRule.Repeated)
                    {
                        // Always output repeated fields with []
                        fieldToStringCode = string.Format("{1}\"[\" + ({0} != null ? string.Join(\", \", {0}.ConvertAll<string>(o => o.ToString()).ToArray()) : \"\") + \"]\"{2}", f.CsName, fieldHeaderCode, fieldCommaCode);
                    } 
                    else
                    {
                        fieldToStringCode = fieldHeaderCode + f.CsName + fieldCommaCode;
                    }
                    fieldElements.Add(fieldToStringCode);
                }
                returnStatement = "return " + string.Join(" + \n", fieldElements) + ";";
            }
            cw.WriteLine(returnStatement);
            cw.EndBracket();


            //Wire format field ID
#if DEBUG
            cw.Comment("ProtocolBuffers wire field id");
            foreach (Field f in m.Fields.Values)
            {
                cw.WriteLine("public const int " + f.CsName + "FieldID = " + f.ID + ";");
            }
#endif
        }

        static string GenerateProperty(Field f)
        {
            string type = f.ProtoType.FullCsType;
            if (f.OptionCodeType != null)
                type = f.OptionCodeType;
            if (f.Rule == FieldRule.Repeated)
                type = "List<" + type + ">";

            if (f.OptionReadOnly)
                return f.OptionAccess + " readonly " + type + " " + f.CsName + " = new " + type + "();";
            else if (f.ProtoType is ProtoMessage && f.ProtoType.OptionType == "struct")
                return f.OptionAccess + " " + type + " " + f.CsName + ";";
            else
                return f.OptionAccess + " " + type + " " + f.CsName + " { get; set; }";
        }
    }
}

