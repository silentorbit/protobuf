using System;

namespace ProtocolBuffers
{
    /// <summary>
    /// This is currently not used.
    /// This approach place all fields and template methods in a base class.
    /// </summary>
    class MessageBaseCode : MessageCode
    {
        public override void GenerateClass(Message m, CodeWriter cw)
        {
            //Base class
            cw.Bracket(m.OptionAccess + " abstract class " + m.CSType + "Base");
            GenerateProperties(m, cw);

            if (m.OptionTriggers)
            {
                cw.Bracket("protected virtual void BeforeSerialize()");
                cw.EndBracket();
                cw.WriteLine();
                cw.Bracket("protected virtual void AfterDeserialize()");
                cw.EndBracket();
            }
            cw.EndBracket();
            cw.WriteLine();
            
            //Default class
            cw.Bracket(m.OptionAccess + " partial class " + m.CSType + " : " + m.CSType + "Base");
            GenerateEnums(m, cw);

            #if !GENERATE_BASE
            GenerateProperties(m, cw);
            #endif
            
            foreach (Message sub in m.Messages)
            {
                cw.WriteLine();
                GenerateClass(sub, cw);
            }
            cw.EndBracket();
            
            return;
        }
        
        protected override string GenerateProperty(Field f)
        {
            return f.OptionAccess + " virtual " + f.PropertyType + " " + f.Name + " { get; set; }";
        }
    }
}

