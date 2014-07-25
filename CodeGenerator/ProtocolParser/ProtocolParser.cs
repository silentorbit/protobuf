using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

// 
//  Read/Write string and byte arrays 
// 
namespace SilentOrbit.ProtocolBuffers
{
    public static partial class ProtocolParser
    {
        public static string ReadString(Stream stream)
        {
            return Encoding.UTF8.GetString(ReadBytes(stream));
        }

        /// <summary>
        /// Reads a length delimited byte array
        /// </summary>
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

        /// <summary>
        /// Skip the next varint length prefixed bytes.
        /// Alternative to ReadBytes when the data is not of interest.
        /// </summary>
        public static void SkipBytes(Stream stream)
        {
            int length = (int)ReadUInt32(stream);
            if (stream.CanSeek)
                stream.Seek(length, SeekOrigin.Current);
            else
                ReadBytes(stream);
        }

        public static void WriteString(Stream stream, string val)
        {
            WriteBytes(stream, Encoding.UTF8.GetBytes(val));
        }

        /// <summary>
        /// Writes length delimited byte array
        /// </summary>
        public static void WriteBytes(Stream stream, byte[] val)
        {
            WriteUInt32(stream, (uint)val.Length);
            stream.Write(val, 0, val.Length);
        }

    }

    /// <summary>
    /// Wrapper for streams that does not support the Position property
    /// </summary>
    public class StreamRead : Stream
    {
        Stream stream;

        /// <summary>
        /// Bytes left to read
        /// </summary>
        public int BytesRead { get; private set; }

        /// <summary>
        /// Define how many bytes are allowed to read
        /// </summary>
        /// <param name='baseStream'>
        /// Base stream.
        /// </param>
        /// <param name='maxLength'>
        /// Max length allowed to read from the stream.
        /// </param>
        public StreamRead(Stream baseStream)
        {
            this.stream = baseStream;
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = stream.Read(buffer, offset, count);
            BytesRead += read;
            return read;
        }

        public override int ReadByte()
        {
            int b = stream.ReadByte();
            BytesRead += 1;
            return b;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.BytesRead;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            stream.Dispose();
            base.Dispose(disposing);
        }
    }
}

