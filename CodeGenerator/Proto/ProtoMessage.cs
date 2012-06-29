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
        public Dictionary<string,ProtoMessage> Messages = new Dictionary<string, ProtoMessage>();
        public Dictionary<string,ProtoEnum> Enums = new Dictionary<string, ProtoEnum>();

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

        public ProtoMessage(ProtoMessage parent, string package)
        {
            if ((parent == null) && (this is ProtoCollection == false))
                throw new ArgumentNullException("parent");
            this.Parent = parent;
            this.Package = package;
            this.OptionType = "class";
        }
        
        public override string ToString()
        {
            return "message " + FullProtoName;
        }

        public bool IsUsingBinaryWriter
        {
            get
            {
                foreach (Field f in Fields.Values)
                    if (f.IsUsingBinaryWriter)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// If all fields are constant then this messag eis constant too
        /// </summary>
        public override int WireSize
        {
            get
            {
                int totalSize = 0;
                foreach (Field f in Fields.Values)
                {
                    if(f.ProtoType.WireSize < 0)
                        return -1;
                    totalSize += f.ProtoType.WireSize;
                }
                return totalSize;
            }
        }

        #region Name searching

        /// <summary>
        /// Search for message in hierarchy
        /// </summary>
        public ProtoType GetProtoType(string path)
        {
            if (this is ProtoCollection)
                return SearchMessage(this, path);

            //Get protocollection, top parent
            ProtoMessage topParent = Parent;
            while (topParent is ProtoCollection == false)
                topParent = topParent.Parent;

            //Search for message or enum
            ProtoType pt;

            //First search down current message hierarchy
            pt = SearchMessage(topParent, Package + "." + ProtoName + "." + path);
            if (pt != null)
                return pt;

            //Second Search local namespace
            pt = SearchMessage(topParent, Package + "." + path);
            if (pt != null)
                return pt;

            //Finally search for global namespace
            return SearchMessage(topParent, path);
        }

        ProtoType SearchMessage(ProtoMessage msg, string fullPath)
        {
            foreach (ProtoMessage sub in msg.Messages.Values)
            {
                if (fullPath == sub.FullProtoName)
                    return sub;

                if (fullPath.StartsWith(sub.FullProtoName + "."))
                {
                    ProtoType pt = SearchMessage(sub, fullPath);
                    if (pt != null)
                        return pt;
                }
            }

            foreach (ProtoEnum subEnum in msg.Enums.Values)
            {
                if (fullPath == subEnum.FullProtoName)
                    return subEnum;
            }

            return null;
        }

        #endregion

    }
}

