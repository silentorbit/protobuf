using System;
using System.IO;

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
		
		public static byte[] ReadVarIntBytes (Stream stream)
		{
			byte[] buffer = new byte[10];
			int offset = 0;
			while (true) {
				int b = stream.ReadByte ();
				if (b < 0)
					throw new IOException ("Stream ended too early");
				buffer [offset] = (byte)b;
				offset += 1;
				if ((b & 0x80) == 0)
					break; //end of varint
				if (offset >= buffer.Length)
					throw new InvalidDataException ("VarInt too long, more than 10 bytes");
			}
			byte[] ret = new byte[offset];
			Array.Copy (buffer, ret, ret.Length);
			return ret;
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
