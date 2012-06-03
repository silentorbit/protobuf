using System;

namespace ProtocolBuffers
{
    /// <summary>
    /// This is not currently used.
    /// It represent a different approach working against interfaces.
    /// This does require all fields to have access public or internal.
    /// </summary>
    class MessageInterfaceCode : MessageCode
    {
        public override void GenerateClass(Message m, CodeWriter cw)
        {
            GenerateInterface(m, cw);
            cw.WriteLine();

            //Default class
            cw.Bracket("public partial class " + m.CSType + " : I" + m.CSType);
            GenerateEnums(m, cw);

            GenerateProperties(m, cw);
            
            foreach (Message sub in m.Messages)
            {
                cw.WriteLine();
                GenerateClass(sub, cw);
            }
            cw.EndBracket();
        }
        
        private void GenerateInterface(Message m, CodeWriter cw)
        {
            cw.Bracket("public interface I" + m.CSType);

            foreach (Field f in m.Fields.Values)
            {
                if (f.OptionDeprecated)
                    cw.WriteLine("[Obsolete]");
                cw.WriteLine(f.PropertyType + " " + f.Name + " { get; set; }");
            }

            cw.EndBracket();
        }

    }
}

