//
//  Reader/Writer for field key
//
using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    public enum Wire
    {
        Varint = 0,
        //int32, int64, UInt32, UInt64, SInt32, SInt64, bool, enum
        Fixed64 = 1,
        //fixed64, sfixed64, double
        LengthDelimited = 2,
        //string, bytes, embedded messages, packed repeated fields
        //Start = 3,        //  groups (deprecated)
        //End = 4,      //  groups (deprecated)
        Fixed32 = 5,
        //32-bit    fixed32, SFixed32, float
    }

    public class Key
    {
        public uint Field { get; set; }

        public Wire WireType { get; set; }

        public Key(uint field, Wire wireType)
        {
            this.Field = field;
            this.WireType = wireType;
        }

        public override string ToString()
        {
            return string.Format("[Key: {0}, {1}]", Field, WireType);
        }
    }

    /// <summary>
    /// Storage of unknown fields
    /// </summary>
    public class KeyValue
    {
        public Key Key { get; set; }

        public byte[] Value { get; set; }

        public KeyValue(Key key, byte[] value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return string.Format("[KeyValue: {0}, {1}, {2} bytes]", Key.Field, Key.WireType, Value.Length);
        }
    }

    public static partial class ProtocolParser
    {

        public static Key ReadKey(Stream stream)
        {
            uint n = ReadUInt32(stream);
            return new Key(n >> 3, (Wire)(n & 0x07));
        }

        public static Key ReadKey(byte firstByte, Stream stream)
        {
            if (firstByte < 128)
                return new Key((uint)(firstByte >> 3), (Wire)(firstByte & 0x07));
            uint fieldID = ((uint)ReadUInt32(stream) << 4) | ((uint)(firstByte >> 3) & 0x0F);
            return new Key(fieldID, (Wire)(firstByte & 0x07));
        }

        public static void WriteKey(Stream stream, Key key)
        {
            uint n = (key.Field << 3) | ((uint)key.WireType);
            WriteUInt32(stream, n);
        }

        /// <summary>
        /// Seek past the value for the previously read key.
        /// </summary>
        public static void SkipKey(Stream stream, Key key)
        {
            switch (key.WireType)
            {
                case Wire.Fixed32:
                    stream.Seek(4, SeekOrigin.Current);
                    return;
                case Wire.Fixed64:
                    stream.Seek(8, SeekOrigin.Current);
                    return;
                case Wire.LengthDelimited:
                    stream.Seek(ProtocolParser.ReadUInt32(stream), SeekOrigin.Current);
                    return;
                case Wire.Varint:
                    ProtocolParser.ReadSkipVarInt(stream);
                    return;
                default:
                    throw new NotImplementedException("Unknown wire type: " + key.WireType);
            }
        }

        /// <summary>
        /// Read the value for an unknown key as bytes.
        /// Used to preserve unknown keys during deserialization.
        /// Requires the message option preserveunknown=true.
        /// </summary>
        public static byte[] ReadValueBytes(Stream stream, Key key)
        {
            byte[] b;
            int offset = 0;

            switch (key.WireType)
            {
                case Wire.Fixed32:
                    b = new byte[4];
                    while (offset < 4)
                        offset += stream.Read(b, offset, 4 - offset);
                    return b;
                case Wire.Fixed64:
                    b = new byte[8];
                    while (offset < 8)
                        offset += stream.Read(b, offset, 8 - offset);
                    return b;
                case Wire.LengthDelimited:
                    //Read and include length in value buffer
                    uint length = ProtocolParser.ReadUInt32(stream);
                    using (var ms = new MemoryStream())
                    {
                        //TODO: pass b directly to MemoryStream constructor or skip usage of it completely
                        ProtocolParser.WriteUInt32(ms, length);
                        b = new byte[length + ms.Length];
                        ms.ToArray().CopyTo(b, 0);
                        offset = (int)ms.Length;
                    }

                    //Read data into buffer
                    while (offset < b.Length)
                        offset += stream.Read(b, offset, b.Length - offset);
                    return b;
                case Wire.Varint:
                    return ProtocolParser.ReadVarIntBytes(stream);
                default:
                    throw new NotImplementedException("Unknown wire type: " + key.WireType);
            }
        }
    }
}

