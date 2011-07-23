using System;
using System.IO;
using System.Text;

// 
//	Read/Write string and byte arrays 
// 
namespace ProtocolBuffers
{
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

