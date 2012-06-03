using System;

namespace ProtocolBuffers
{
	/// <summary>
	/// Types in .proto files plus message and enum
	/// </summary>
	enum ProtoTypes
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
	
}

