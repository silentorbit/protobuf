using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    class ProtoEnum : ProtoType, IComment
    {
        public override Wire WireType
        {
            get { return Wire.Varint; }
        }

        public string Comments { get; set; }
        public Dictionary<string,int> Enums = new Dictionary<string, int>();
        public Dictionary<string,string> EnumsComments = new Dictionary<string, string>();
        
        public ProtoEnum(ProtoMessage parent, string package) : base(parent, package)
        {
        }
    }
}

