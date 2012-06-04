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

        public Wire WireType
        {
            get
            {
                if(OptionPacked)
                    return Wire.LengthDelimited;
                return ProtoType.WireType;
            }
        }
        
        #region Code Generation Properties
        
        //These are generated as a second stage parsing of the .proto file.
        //They are used in the code generation.
        
        /// <summary>
        /// .proto type including enum and message.
        /// </summary>
        public ProtoType ProtoType { get; set; }

        #endregion
        
        public override string ToString()
        {
            return string.Format("{0} {1} {2} = {3}", Rule, ProtoTypeName, Name, ID);
        }
    }
}

