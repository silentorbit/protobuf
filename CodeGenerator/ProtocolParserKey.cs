//
//	Reader/Writer for field key
//
using System;
using System.IO;

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

