using System;

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
		//Max = 7
	}
}

