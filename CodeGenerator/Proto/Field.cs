using System;

namespace ProtocolBuffers
{
	public class Field
	{
		public Field ()
		{
			this.OptionAccess = "public";
		}
		
		#region .proto data
		
		public Rules Rule { get; set; }
		
		/// <summary>
		/// As read from the .proto file
		/// </summary>
		public string ProtoTypeName { get; set; }
		
		public string Name { get; set; }

		public uint ID { get; set; }
			
		//Field options
		public bool OptionPacked = false;
		public bool OptionDeprecated = false;
		public string OptionDefault = null;
		
		#region Locally used fields
		
		/// <summary>
		/// Local for this implementation.
		/// define the access of the field: public(default), protected, private or internal
		/// </summary>
		public string OptionAccess = "public";
		
		/// <summary>
		/// Define the type of the property that is not a primitive or class derived from a message.
		/// This can be one of the build in (DateTime, TimeSpan) or a custom class that implements the static Read and Write functions;
		/// </summary>
		public string OptionCustomType = null;
		
		/// <summary>
		/// Generate property in class, if not it is expected to already be defined elsewhere.
		/// </summary>
		public bool OptionGenerate = true;
		
		#endregion
		
		#endregion
		
		#region Code Generation Properties
		
		//These are generated as a second stage parsing of the .proto file.
		//They are used in the code generation.
		
		/// <summary>
		/// .proto type includng enum and message.
		/// </summary>
		public ProtoTypes ProtoType { get; set; }
		
		/// <summary>
		/// If a message type this point to the Message class, for use in code generation
		/// </summary>
		public Message ProtoTypeMessage { get; set; }
		
		/// <summary>
		/// If an enum type this point to the MessageEnum class, for use in code generation
		/// </summary>
		public MessageEnum ProtoTypeEnum { get; set; }
		
		/// <summary>
		/// Based on Prototype and Rule according to the protocol buffers specification
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

		public string FullPath {
			get {
				IProtoType pt;
				if (ProtoType == ProtoTypes.Message)
					pt = ProtoTypeMessage;
				else if (ProtoType == ProtoTypes.Enum)
					pt = ProtoTypeEnum;
				else
					throw new InvalidOperationException ();
			
				return pt.Namespace + "." + this.CSClass;
			}
		}
		
		/// <summary>
		/// Generate full Interface path
		/// </summary>
		public string PropertyType {
			get {
				if (Rule == Rules.Repeated)
					return "List<" + PropertyItemType + ">";
				else
					return PropertyItemType;
			}
		}

		/// <summary>
		/// Generate full Interface path
		/// </summary>
		public string PropertyItemType {
			get {
				if (OptionCustomType != null)
					return OptionCustomType;
			
				switch (ProtoType) {
				case ProtoTypes.Message:
					return ProtoTypeMessage.FullPath + "." + CSType;
				case ProtoTypes.Enum:
					string path = CSType;
					Message message = ProtoTypeEnum.Parent;
					if (message is Proto == false)
						path = message.CSName + "." + path;
					return message.FullPath + "." + path;
				default:	
					return CSType;
				}
			}
		}

		#endregion
		
		public override string ToString ()
		{
			return string.Format ("{0} {1} {2} = {3}", Rule, ProtoTypeName, Name, ID);
		}
	}
}

