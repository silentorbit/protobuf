using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	
	/// <summary>
	/// Representation of a .proto file
	/// </summary>
	public class Proto : Message
	{
		public Proto () : base(null)
		{			
		}
		
		public override string ToString ()
		{
			string t = "Proto: ";
			foreach (Message m in Messages)
				t += "\n\t" + m;
			return t;
		}
	}
	
	public interface IProtoType
	{
		Message Parent { get; set; }

		string CSName { get; set; }
	}
	
	public class Message : IProtoType
	{
		public Message Parent { get; set; }

		public string ProtoName { get; set; }

		public string CSName { get; set; }

		public List<Field> Fields { get; set; }

		public List<Message> Messages { get; set; }

		public List<MessageEnum> Enums { get; set; }
	
		public Dictionary<string,string> Options { get; set; }
			
		public Message (Message parent)
		{
			this.Parent = parent;
			this.Fields = new List<Field> ();
			this.Messages = new List<Message> ();
			this.Enums = new List<MessageEnum> ();
			this.Options = new Dictionary<string, string> ();
		}
		
		public override string ToString ()
		{
			return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
		}
	}
			
	public enum Wire
	{
		Varint = 0,		//int32, int64, uint32, uint64, sint32, sint64, bool, enum
		Fixed64 = 1,	//fixed64, sfixed64, double
		LengthDelimited = 2,	//string, bytes, embedded messages, packed repeated fields
		//Start = 3, 		//	groups (deprecated)
		//End = 4,		//	groups (deprecated)
		Fixed32 = 5,	//32-bit	fixed32, sfixed32, float
	}
	
	public class Field
	{
		#region .proto data
		
		public Rules Rule { get; set; }
		
		/// <summary>
		/// As read from the .proto file
		/// </summary>
		public string ProtoTypeName { get; set; }
		
		public string Name { get; set; }

		public uint ID { get; set; }
			
		//Field options
		public bool Packed { get; set; }

		public bool Deprecated { get; set; }
		
		public string Default { get; set; }
		
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

		#endregion
		
		public override string ToString ()
		{
			return string.Format ("{0} {1} {2} = {3}", Rule, ProtoTypeName, Name, ID);
		}
	}
	
	/// <summary>
	/// Rules for fields in .proto files
	/// </summary>
	public enum Rules
	{
		Required,	//a well-formed message must have exactly one of this field.
		Optional,	//a well-formed message can have zero or one of this field (but not more than one).
		Repeated,	//this field can be repeated any number of times (including zero) in a well-formed message. The order of the repeated values will be preserved.
	}
	
	/// <summary>
	/// Types in .proto files plus message and enum
	/// </summary>
	public enum ProtoTypes
	{
		Unknown = 0,
		Double, //		double	double
		Float, //		float	float
		Int32, //	Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint32 instead.	int32	int
		Int64, //	Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint64 instead.	int64	long
		Uint32, //	Uses variable-length encoding.	uint32	int[1]
		Uint64, //	Uses variable-length encoding.	uint64	long[1]
		Sint32, //	Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int32s.	int32	int
		Sint64, //	Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int64s.	int64	long
		Fixed32, //	Always four bytes. More efficient than uint32 if values are often greater than 228.	uint32	int[1]
		Fixed64, //	Always eight bytes. More efficient than uint64 if values are often greater than 256.	uint64	long[1]
		Sfixed32, //	Always four bytes.	int32	int
		Sfixed64, //	Always eight bytes.	int64	long
		Bool, //		bool	boolean
		String, //	A string must always contain UTF-8 encoded or 7-bit ASCII text.	string	String
		Bytes, //	May contain any arbitrary sequence of bytes.	string	ByteString
		
		//Extra used only locally
		Message,
		Enum,
	}
	
	public class MessageEnum : IProtoType
	{
		public Message Parent { get; set; }
	
		public string ProtoName { get; set; }
		
		public string CSName { get; set; }

		public Dictionary<string,int> Enums { get; set; }
		
		public MessageEnum (Message parent)
		{
			this.Parent = parent;
			this.Enums = new Dictionary<string, int> ();
		}
	}
}

