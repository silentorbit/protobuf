using System;
using System.IO;
using System.Text;

// 
//  Read/Write string and byte arrays 
// 
namespace ProtocolBuffers
{
    public static partial class ProtocolParser
    {
        
        public static string ReadString(Stream stream)
        {
            return Encoding.UTF8.GetString(ReadBytes(stream));
        }
        
        public static byte[] ReadBytes(Stream stream)
        {
            //VarInt length
            int length = (int)ReadUInt32(stream);
            
            //Bytes
            byte[] buffer = new byte[length];
            int read = 0;
            while (read < length)
            {
                int r = stream.Read(buffer, read, length - read);
                if (r == 0)
                    throw new InvalidDataException("Expected " + (length - read) + " got " + read);
                read += r;
            }
            return buffer;
        }
        
        public static void WriteString(Stream stream, string val)
        {
            WriteBytes(stream, Encoding.UTF8.GetBytes(val));
        }
        
        public static void WriteBytes(Stream stream, byte[] val)
        {
            WriteUInt32(stream, (uint)val.Length);
            stream.Write(val, 0, val.Length);
        }
        
    }
}

