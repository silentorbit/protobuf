using System;

namespace ProtocolBuffers
{
    class MessageCode
    {
        public virtual void GenerateClass(Message m, CodeWriter cw)
        {
            //Default class
            if (m.Comments != null)
                cw.Summary(m.Comments);
            cw.Bracket(m.OptionAccess + " partial class " + m.CSType);

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
            
            foreach (Message sub in m.Messages)
            {
                GenerateClass(sub, cw);
                cw.WriteLine();
            }
            cw.EndBracket();
            return;
        }

        protected void GenerateEnums(Message m, CodeWriter cw)
        {
            foreach (MessageEnum me in m.Enums)
            {
                cw.Bracket("public enum " + me.CSType);
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
        protected void GenerateProperties(Message m, CodeWriter cw)
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
        
        protected virtual string GenerateProperty(Field f)
        {
            if (f.OptionReadOnly)
                return f.OptionAccess + " readonly " + f.PropertyType + " " + f.Name + " = new " + f.PropertyType + "();";
            else
                return f.OptionAccess + " " + f.PropertyType + " " + f.Name + " { get; set; }";
        }
    }
}

