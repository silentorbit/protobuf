using System;

namespace ProtocolBuffers
{
    class ProtoFormatException : Exception
    {
        public TokenReader Token { get; private set; }
        public CsProtoReader CsProto { get; private set; }

        public ProtoFormatException(string message) : base(message)
        {
        }
        
        public ProtoFormatException(string message, TokenReader reader) : base(message)
        {
            this.Token = reader;
        }

        public ProtoFormatException(string message, CsProtoReader reader) : base(message)
        {
            this.CsProto = reader;
        }        

        public ProtoFormatException(string message, Exception innerException, TokenReader reader) : base(message, innerException)
        {
            this.Token = reader;
        }
    }
}

