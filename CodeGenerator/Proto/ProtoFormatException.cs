using System;

namespace ProtocolBuffers
{
	class ProtoFormatException : Exception
	{
		public ProtoFormatException (string message) : base(message)
		{
		}
		
		public ProtoFormatException (string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

