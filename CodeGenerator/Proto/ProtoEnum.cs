using System;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    class ProtoEnum : ProtoType, IComment
    {
        public override Wire WireType
        {
            get { return Wire.Varint; }
        }

        public string Comments { get; set; }

        public List<ProtoEnumValue> Enums = new List<ProtoEnumValue>();

        public ProtoEnum(ProtoMessage parent, string package)
            : base(parent, package)
        {
        }
    }

    public class ProtoEnumValue
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public string Comment { get; set; }

        public ProtoEnumValue(string name, int value, List<string> comments)
        {
            this.Name = name;
            this.Value = value;
            this.Comment = string.Join("\r\n", comments);
            comments.Clear();
        }
    }
}

