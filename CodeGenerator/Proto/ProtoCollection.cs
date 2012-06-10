using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    
    /// <summary>
    /// Representation content of on or more .proto files
    /// </summary>
    class ProtoCollection : ProtoMessage
    {
        public ProtoCollection() : base(null)
        {
        }

        /// <summary>
        /// Defaults to Example if not specified
        /// </summary>
        public override string CsNamespace
        {
            get
            {
                throw new InvalidOperationException();
            }
        }

        public void Merge(ProtoCollection proto)
        {
            foreach (var m in proto.Messages.Values)
            {
                Messages.Add(m.ProtoName, m);
                m.Parent = this;
            }
            foreach (var e in proto.Enums.Values)
            {
                Enums.Add(e.ProtoName, e);
                e.Parent = this;
            }
        }        

        public override string ToString()
        {
            string t = "ProtoCollection: ";
            foreach (ProtoMessage m in Messages.Values)
                t += "\n\t" + m;
            return t;
        }
    }
    
}

