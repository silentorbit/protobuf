using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    class ProtoMessage : ProtoType
    {
        public override Wire WireType
        {
            get { return Wire.LengthDelimited; }
        }

        public string Comments;
        public Dictionary<int, Field> Fields = new  Dictionary<int, Field>();
        public List<ProtoMessage> Messages = new List<ProtoMessage>();
        public List<ProtoEnum> Enums = new List<ProtoEnum>();

        public string SerializerType
        {
            get
            {
                if (this.OptionExternal || this.OptionType == "interface")
                    return CsType + "Serializer";
                else
                    return CsType;
            }
        }

        public string FullSerializerType
        {
            get
            {
                if (this.OptionExternal || this.OptionType == "interface")
                    return FullCsType + "Serializer";
                else
                    return FullCsType;
            }
        }

        public ProtoMessage(ProtoMessage parent)
        {
            this.Parent = parent;
            this.OptionType = "class";
        }
        
        public override string ToString()
        {
            return string.Format("[ProtoMessage: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
        }
        
    }
}

