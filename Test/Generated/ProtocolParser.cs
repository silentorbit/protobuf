#region ProtocolParser
using System;
using System.IO;
using System.Text;

// 
//	Read/Write string and byte arrays 
// 
namespace ProtocolBuffers
{
	/*
	public static partial class Serializer
	{
		public static byte[] GetBytes<T>(T instance)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				Write(ms, instance);
				return ms.ToArray();
			}
		}
	}*/
	public static partial class ProtocolParser
	{
		
		public static string ReadString (Stream stream)
		{
			return Encoding.UTF8.GetString (ReadBytes (stream));
		}
		
		public static byte[] ReadBytes (Stream stream)
		{
			//VarInt length
			uint length = ReadUInt32 (stream);
			
			//Bytes
			byte[] buffer = new byte[length];
			if (length != 0)
				stream.Read (buffer, 0, buffer.Length);
			return buffer;
		}
		
		public static void WriteString (Stream stream, string val)
		{
			WriteBytes (stream, Encoding.UTF8.GetBytes (val));
		}
		
		public static void WriteBytes (Stream stream, byte[] val)
		{
			WriteUInt32 (stream, (uint)val.Length);
			stream.Write (val, 0, val.Length);
		}
		
	}
}

#endregion
#region ProtocolParserFixed
//
//	This file contain references on how to write and read
//	fixed integers and float/double.
//	

namespace ProtocolBuffers
{
	public static partial class ProtocolParser
	{
		#region Fixed Int
		
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static ulong ReadFixed64 (BinaryReader reader)
		{
			return reader.ReadUInt64 ();
		}

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static long ReadSFixed64 (BinaryReader reader)
		{
			return reader.ReadInt64 ();
		}
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static uint ReadFixed32 (BinaryReader reader)
		{
			return reader.ReadUInt32 ();
		}

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static int ReadSFixed32 (BinaryReader reader)
		{
			return reader.ReadInt32 ();
		}
		
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteFixed64 (BinaryWriter writer, ulong val)
		{
			writer.Write (val);
		}

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteSFixed64 (BinaryWriter writer, long val)
		{
			writer.Write (val);
		}
		
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteFixed32 (BinaryWriter writer, uint val)
		{
			writer.Write (val);
		}

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteSFixed32 (BinaryWriter writer, int val)
		{
			writer.Write (val);
		}
		
		#endregion
		
		#region Fixed: float, double

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static float ReadFloat (BinaryReader reader)
		{
			return reader.ReadSingle ();
		}
		
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static double ReadDouble (BinaryReader reader)
		{
			return reader.ReadDouble ();
		}

		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteFloat (BinaryWriter writer, float val)
		{
			writer.Write (val);
		}
		
		/// <summary>
		/// Only for reference
		/// </summary>
		[Obsolete("Only for reference")]
		public static void WriteDouble (BinaryWriter writer, double val)
		{
			writer.Write (val);
		}


		#endregion
		
	}
}

#endregion
#region ProtocolParserKey
//
//	Reader/Writer for field key
//

namespace ProtocolBuffers
{
	public enum Wire
	{
		Varint = 0,		//int32, int64, uint32, uint64, sint32, sint64, bool, enum
		Fixed64 = 1,	//fixed64, sfixed64, double
		LengthDelimited = 2,	//string, bytes, embedded messages, packed repeated fields
		//Start = 3, 		//	groups (deprecated)
		//End = 4,		//	groups (deprecated)
		Fixed32 = 5,	//32-bit	fixed32, sfixed32, float
	}

	public class Key
	{
		public uint Field { get; set; }

		public Wire WireType { get; set; }
		
		public Key (uint field, Wire wireType)
		{
			this.Field = field;
			this.WireType = wireType;				
		}
	}
	
	public static partial class ProtocolParser
	{
		
		public static Key ReadKey (Stream stream)
		{
			uint n = ReadUInt32 (stream);
			return new Key (n >> 3, (Wire)(n & 0x07));
		}
		
		public static Key ReadKey (byte firstByte, Stream stream)
		{
			if (firstByte < 128)
				return new Key ((uint)(firstByte >> 3), (Wire)(firstByte & 0x07));
			uint n = ReadUInt32 (stream) << 7 | firstByte;
			return new Key (n >> 3, (Wire)(n & 0x07));
		}
		
		public static void WriteKey (Stream stream, Key key)
		{
			uint n = (key.Field << 3) | ((uint)key.WireType);
			WriteUInt32 (stream, n);
		}
		
		public static void SkipKey (Stream stream, Key key)
		{
			switch (key.WireType) {
			case Wire.Fixed32:
				stream.Seek (4, SeekOrigin.Current);
				return;
			case Wire.Fixed64:
				stream.Seek (8, SeekOrigin.Current);
				return;
			case Wire.LengthDelimited:
				stream.Seek (ProtocolParser.ReadUInt32 (stream), SeekOrigin.Current);
				return;
			case Wire.Varint:
				ProtocolParser.ReadSkipVarInt (stream);
				return;
			default:
				throw new NotImplementedException ("Unknown wire type: " + key.WireType);
			}
		}
	}
}

#endregion
#region ProtocolParserVarInt

namespace ProtocolBuffers
{
	public static partial class ProtocolParser
	{
		/// <summary>
		/// Reads past a varint for an unknown field.
		/// </summary>
		public static void ReadSkipVarInt (Stream stream)
		{
			while (true) {
				int b = stream.ReadByte ();
				if (b < 0)
					throw new IOException ("Stream ended too early");
				
				if ((b & 0x80) == 0)
					return; //end of varint
			}
		}
		
		#region VarInt: int32, uint32, sint32
		
		[Obsolete("Use (int)ReadUInt32 (stream);")]
		/// <summary>
		/// Since the int32 format is inefficient for negative numbers we have avoided to imlplement.
		/// The same functionality can be achieved using: (int)ReadUInt32 (stream);
		/// </summary>
		public static int ReadInt32 (Stream stream)
		{
			throw new NotImplementedException ("Use (int)ReadUInt32 (stream);");
		}
		
		[Obsolete("Use WriteUInt32 (stream, (uint)val);")]
		/// <summary>
		/// Since the int32 format is inefficient for negative numbers we have avoided to imlplement.
		/// The same functionality can be achieved using: WriteUInt32 (stream, (uint)val);
		/// </summary>
		public static void WriteInt32 (Stream stream, int val)
		{
			throw new NotImplementedException ("Use WriteUInt32 (stream, (uint)val);");
		}
		
		public static int ReadSInt32 (Stream stream)
		{
			uint val = ReadUInt32 (stream);
			return (int)(val >> 1) ^ ((int)(val << 31) >> 31);
		}
		
		public static void WriteSInt32 (Stream stream, int val)
		{
			WriteUInt32 (stream, (uint)((val << 1) ^ (val >> 31)));
		}

		public static uint ReadUInt32 (Stream stream)
		{
			int b;
			uint val = 0;
			
			for (int n = 0; n < 5; n++) {
				b = stream.ReadByte ();
				if (b < 0)
					throw new IOException ("Stream ended too early");
				
				//Check that it fits in 32 bits
				if ((n == 4) && (b & 0xF0) != 0)
					throw new InvalidDataException ("Got larger VarInt than 32bit unsigned");
				//End of check
				
				if ((b & 0x80) == 0)
					return val | (uint)b << (7 * n);
				
				val |= (uint)(b & 0x7F) << (7 * n);
			}
			
			throw new InvalidDataException ("Got larger VarInt than 32bit unsigned");
		}
		
		public static void WriteUInt32 (Stream stream, uint val)
		{
			byte[] buffer = new byte[5];
			int count = 0;
			
			while (true) {
				buffer [count] = (byte)(val & 0x7F);
				val = val >> 7;
				if (val == 0)
					break;
				
				buffer [count] |= 0x80;
				
				count += 1;
			}
			
			stream.Write (buffer, 0, count + 1);
		}
		
		#endregion
		
		#region VarInt: int64, uint64, sint64
		
		[Obsolete("Use (long)ReadUInt64 (stream); instead")]
		/// <summary>
		/// Since the int64 format is inefficient for negative numbers we have avoided to implement it.
		/// The same functionality can be achieved using: (long)ReadUInt64 (stream);
		/// </summary>
		public static int ReadInt64 (Stream stream)
		{
			throw new NotImplementedException ("Use (int)ReadUInt64 (stream); instead");
		}
		
		[Obsolete("Use WriteUInt64 (stream, (ulong)val); instead")]
		/// <summary>
		/// Since the int64 format is inefficient for negative numbers we have avoided to implement.
		/// The same functionality can be achieved using: WriteUInt64 (stream, (ulong)val);
		/// </summary>
		public static void WriteInt64 (Stream stream, int val)
		{
			throw new NotImplementedException ("Use WriteUInt64 (stream, (ulong)val); instead");
		}

		public static long ReadSInt64 (Stream stream)
		{
			ulong val = ReadUInt64 (stream);
			return (long)(val >> 1) ^ ((long)(val << 63) >> 63);
		}
		
		public static void WriteSInt64 (Stream stream, long val)
		{
			WriteUInt64 (stream, (ulong)((val << 1) ^ (val >> 63)));
		}

		public static ulong ReadUInt64 (Stream stream)
		{
			int b;
			ulong val = 0;
			
			for (int n = 0; n < 10; n++) {
				b = stream.ReadByte ();
				if (b < 0)
					throw new IOException ("Stream ended too early");
				
				//Check that it fits in 64 bits
				if ((n == 9) && (b & 0xFE) != 0)
					throw new InvalidDataException ("Got larger VarInt than 64 bit unsigned");
				//End of check
				
				if ((b & 0x80) == 0)
					return val | (ulong)b << (7 * n);
				
				val |= (ulong)(b & 0x7F) << (7 * n);
			}
			
			throw new InvalidDataException ("Got larger VarInt than 64 bit unsigned");
		}
		
		public static void WriteUInt64 (Stream stream, ulong val)
		{
			byte[] buffer = new byte[10];
			int count = 0;
			
			while (true) {
				buffer [count] = (byte)(val & 0x7F);
				val = val >> 7;
				if (val == 0)
					break;
				
				buffer [count] |= 0x80;
				
				count += 1;
			}
			
			stream.Write (buffer, 0, count + 1);
		}
		
		#endregion
		
		#region Varint: bool
		
		public static bool ReadBool (Stream stream)
		{
			int b = stream.ReadByte ();
			if (b < 0)
				throw new IOException ("Stream ended too early");
			if (b == 1)
				return true;
			if (b == 0)
				return false;
			throw new InvalidDataException ("Invalid boolean value");
		}
		
		public static void WriteBool (Stream stream, bool val)
		{
			stream.WriteByte (val ? (byte)1 : (byte)0);
		}
		
		#endregion
	}
}
#endregion
#region ProtocolParserCustom

namespace ProtocolBuffers
{
	public static partial class ProtocolParser
	{

	}
}

#endregion
