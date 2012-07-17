using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    class ProtoEnum : ProtoType
    {
        public override Wire WireType
        {
            get { return Wire.Varint; }
        }

        public string Comments;
        public Dictionary<string,int> Enums = new Dictionary<string, int>();
        public Dictionary<string,string> EnumsComments = new Dictionary<string, string>();
        
        public ProtoEnum(ProtoMessage parent, string package)
        {
            this.Parent = parent;
            if ((parent == null))
                throw new ArgumentNullException("parent");
            this.Package = package;
        }
    }
}

