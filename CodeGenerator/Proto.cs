using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	/// <summary>
	/// Representation of a .proto file
	/// </summary>
	public class Proto
	{
		public Dictionary<string, Message> Messages { get; set; }
		
		public Proto ()
		{
			this.Messages = new Dictionary<string, Message> ();
		}
		
		public override string ToString ()
		{
			string t = "Proto: ";
			foreach (var kvp in Messages)
				t += "\n\t" + kvp.Value;
			return t;
		}
	}
	
	public class Message
	{
		public string Name { get; set; }

		public List<Field> Fields { get; set; }

		public Dictionary<string, MessageEnum> Enums { get; set; }
		
		public Message ()
		{
			this.Fields = new List<Field> ();
			this.Enums = new Dictionary<string, MessageEnum> ();
		}
		
		public override string ToString ()
		{
			return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", Name, Fields.Count, Enums.Count);
		}
	}
	
	public class Field
	{
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
		
		///Used later in CodeGeneration
		
		public ProtoTypes ProtoType { get; set; }
		
		public Wire WireType { get; set; }
		
		public string CSType { get; set; }

		public string CSItemType { get; set; }

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
	
	public class MessageEnum
	{
		public string Name { get; set; }

		public Dictionary<string,int> Enums { get; set; }
		
		public MessageEnum ()
		{
			this.Enums = new Dictionary<string, int> ();
		}
	}
}

