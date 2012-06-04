using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    
    /// <summary>
    /// Representation of a .proto file
    /// </summary>
    class ProtoFile : ProtoMessage
    {
        public ProtoFile() : base(null)
        {
        }
        
        /// <summary>
        /// Defaults to Example if not specified
        /// </summary>
        public override string CsNamespace
        {
            get
            {
                if (OptionNamespace == null)
                    return "Example";
                return OptionNamespace;
            }
        }
        
        public override string ToString()
        {
            string t = "Proto: ";
            foreach (ProtoMessage m in Messages)
                t += "\n\t" + m;
            return t;
        }
    }
    
}

