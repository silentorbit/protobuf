using System;

namespace ProtocolBuffers
{
    /// <summary>
    /// A protobuf message or enum
    /// </summary>
    abstract class ProtoType
    {
        public ProtoMessage Parent { get; set; }

        /// <summary>
        /// Name used in the .proto file, 
        /// </summary>
        public string ProtoName { get; set; }
    
        /// <summary>
        /// Based on ProtoType and Rule according to the protocol buffers specification
        /// </summary>
        public abstract Wire WireType { get; }

        /// <summary>
        /// The c# type name
        /// </summary>
        public virtual string CsType { get; set; }

        /// <summary>
        /// The C# namespace for this item
        /// </summary>
        public virtual string CsNamespace
        {
            get
            {
                if (OptionNamespace == null)
                {
                    if (Parent is ProtoFile)
                        return Parent.CsNamespace;
                    else
                        return Parent.CsNamespace + "." + Parent.CsType;
                } else 
                    return OptionNamespace;
            }
        }

        public virtual string FullCsType
        {
            get { return CsNamespace + "." + CsType;}
        }

        #region Local options

        public string OptionNamespace { get; set; }

        /// <summary>
        /// (C#) access modifier: public(default)/protected/private
        /// </summary>
        public string OptionAccess  { get; set; }
        
        /// <summary>
        /// Call triggers before/after serialization.
        /// </summary>
        public bool OptionTriggers { get; set; }

        /// <summary>
        /// Keep unknown fields when deserializing and send them back when serializing.
        /// This will generate field to store any unknown keys and their value.
        /// </summary>
        public bool OptionPreserveUnknown { get; set; }

        /// <summary>
        /// Don't create class/struct, only serializing code, useful when serializing types in external DLL
        /// </summary>
        public bool OptionExternal { get; set; }

        /// <summary>
        /// Assume code for this message is already generated elsewhere
        /// </summary>
        public bool OptionImported { get; set; }

        /// <summary>
        /// Can be "class", "struct" or "interface"
        /// </summary>
        public string OptionType { get; set; }

        #endregion

        public ProtoType()
        {
            this.OptionNamespace = null;
            this.OptionAccess = "public";
            this.OptionTriggers = false;
            this.OptionPreserveUnknown = false;
            this.OptionExternal = false;
            this.OptionImported = false;
            this.OptionType = null;
        }

        public bool Nullable
        {
            get
            {
                if (ProtoName == ProtoBuiltin.String)
                    return true;
                if (ProtoName == ProtoBuiltin.Bytes)
                    return true;
                if (this is ProtoMessage)
                {
                    if (OptionType == "class")
                        return true;
                    if (OptionType == "interface")
                        return true;
                }
                return false;
            }
        }
    }
}

