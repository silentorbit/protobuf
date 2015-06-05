using System;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// Representation content of one or more .proto files
    /// </summary>
    class ProtoCollection : ProtoMessage
    {
        public List<string> Import = new List<string>();
        public List<string> ImportPublic = new List<string>();

        public ProtoCollection()
            : base(null, null)
        {
        }

        /// <summary>
        /// Defaults to Example if not specified
        /// </summary>
        public override string CsNamespace
        {
            get
            {
                throw new InvalidOperationException("This is a collection of multiple .proto files with different namespaces, namespace should have been set at local.");
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

