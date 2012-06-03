using System;

namespace ProtocolBuffers
{
    class Field
    {
        #region .proto data
        
        public string Comments;
        
        public FieldRule Rule { get; set; }
        
        /// <summary>
        /// As read from the .proto file
        /// </summary>
        public string ProtoTypeName { get; set; }
        
        public string Name { get; set; }

        public int ID { get; set; }
            
        //Field options
        public bool OptionPacked = false;
        public bool OptionDeprecated = false;
        public string OptionDefault = null;
        
        #region Locally used fields
        
        //These options are not the build in ones and have a meaning in the code generation
        
        /// <summary>
        /// Define the access of the field: public, protected, private or internal
        /// </summary>
        public string OptionAccess = "public";
        
        /// <summary>
        /// <para>Define the type of the property that is not a primitive or class derived from a message.</para>
        /// <para>This can be one of the build in (see method MessageCode.GenerateFieldTypeWriter()) or a custom class that implements the static Serialize and Deserialize functions;</para>
        /// </summary>
        public string OptionCodeType = null;
        
        /// <summary>
        /// Generate property in class, if not it is expected to already be defined elsewhere.
        /// </summary>
        public bool OptionGenerate = true;
        
        /// <summary>
        /// Field is (c#)readonly.
        /// Can be set to true if OptionGenerate=false and your own code 
        /// </summary>
        public bool OptionReadOnly = false;
        
        #endregion //Local options
        
        #endregion //.proto data
        
        #region Code Generation Properties
        
        //These are generated as a second stage parsing of the .proto file.
        //They are used in the code generation.
        
        /// <summary>
        /// .proto type including enum and message.
        /// </summary>
        public ProtoTypes ProtoType { get; set; }
        
        /// <summary>
        /// If a message type this point to the Message class
        /// </summary>
        public Message ProtoTypeMessage { get; set; }
        
        /// <summary>
        /// If an enum type this point to the MessageEnum class
        /// </summary>
        public MessageEnum ProtoTypeEnum { get; set; }
        
        /// <summary>
        /// Based on ProtoType and Rule according to the protocol buffers specification
        /// </summary>
        public Wire WireType { get; set; }
        
        /// <summary>
        /// C# type, interface
        /// </summary>
        public string CSType { get; set; }
        
        /// <summary>
        /// C# class of the default class, useful with new Class() expressions.
        /// </summary>
        public string CSClass { get; set; }

        public string FullPath
        {
            get
            {
                MessageEnumBase pt;
                if (ProtoType == ProtoTypes.Message)
                    pt = ProtoTypeMessage;
                else if (ProtoType == ProtoTypes.Enum)
                    pt = ProtoTypeEnum;
                else
                    throw new InvalidOperationException();
            
                return pt.Namespace + "." + pt.CSType;
            }
        }
        
        /// <summary>
        /// Generate full Interface path
        /// </summary>
        public string PropertyType
        {
            get
            {
                if (Rule == FieldRule.Repeated)
                    return "List<" + PropertyItemType + ">";
                else
                    return PropertyItemType;
            }
        }

        /// <summary>
        /// Generate full Interface path
        /// </summary>
        public string PropertyItemType
        {
            get
            {
                if (OptionCodeType != null)
                    return OptionCodeType;
            
                switch (ProtoType)
                {
                    case ProtoTypes.Message:
                        return ProtoTypeMessage.FullCSType;
                    case ProtoTypes.Enum:
                        return ProtoTypeEnum.FullCSType;
                    default:    
                        return CSType;
                }
            }
        }

        #endregion
        
        public override string ToString()
        {
            return string.Format("{0} {1} {2} = {3}", Rule, ProtoTypeName, Name, ID);
        }
    }
}

