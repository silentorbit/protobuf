using System;

namespace ProtocolBuffers
{
    class ProtoPacked : ProtoType
    {
        public ProtoType Sub { get; set; }

        public override Wire WireType
        {
            get { return Wire.LengthDelimited; }
        }

        public ProtoPacked(ProtoType subType)
        {
            this.Sub = subType;
        }
    }
}

