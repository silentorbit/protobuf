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

        public ProtoMessage(ProtoMessage parent)
        {
            if((parent == null) && (this is ProtoCollection == false))
                throw new ArgumentNullException("parent");
            this.Parent = parent;
            this.OptionType = "class";
        }
        
        public override string ToString()
        {
            return "message " + FullProtoName;
        }

        
        #region Name searching

        /// <summary>
        /// Search for field name in message hierarchy
        /// </summary>
        public ProtoType GetProtoType(string path)
        {
            //First assume local path
            string[] parts = path.Split('.');
            ProtoType pt = SearchMessageUp(parts);
            if(pt != null)
                return pt;

            //If not found test by adding package name as prefix
            parts = (Package + "." + path).Split('.');
            return SearchMessageUp(parts);
        }

        /// <summary>
        /// Searchs the message for matchink classes
        /// </summary>
        /// <param name='name'>
        /// name from .proto
        /// </param>
        ProtoType SearchMessageUp(string[] name)
        {
            if (this is ProtoCollection)
                return SearchMessageDown(name);
            
            if (ProtoName == name [0])
            {
                if (name.Length == 1)
                    return this;
                
                string[] subName = new string[name.Length - 1];
                Array.Copy(name, 1, subName, 0, subName.Length);
                
                return SearchMessageDown(subName);
            }
            
            ProtoType down = SearchMessageDown(name);
            if (down != null)
                return down;
            
            return Parent.SearchMessageUp(name);
        }
        
        /// <summary>
        /// Search down for matching name
        /// </summary>
        /// <param name='name'>
        /// Split .proto type name
        /// </param>
        ProtoType SearchMessageDown(string[] name)
        {
            var p = this as ProtoMessage;
            if (name.Length == 1)
            {
                foreach (ProtoEnum me in p.Enums.Values)
                {
                    if (me.ProtoName == name [0])
                        return me;
                }
            }

            int index = 0;
            if (this is ProtoCollection)
            {
                if(name.Length < 2)
                    return null;
                index = 1;
            }

            foreach (ProtoMessage sub in p.Messages.Values)
            {
                if (this is ProtoCollection)
                {
                    if(sub.Package != name[0])
                        continue;
                }

                if (sub.ProtoName == name [index])
                {
                    if (name.Length == 1 + index)
                        return sub;
                    string[] subName = new string[name.Length - 1 - index];
                    Array.Copy(name, 1 + index, subName, 0, subName.Length);
                    
                    return sub.SearchMessageDown(subName);
                }
            }
            
            return null;
        }
        
        #endregion

    }
}

