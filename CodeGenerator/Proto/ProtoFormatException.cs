using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    class ProtoFormatException : Exception
    {
        public readonly SourcePath SourcePath;

        public ProtoFormatException(string message, SourcePath s)
            : base(message)
        {
            this.SourcePath = s;
        }

        public ProtoFormatException(string message, TokenReader tr)
            : base(message)
        {
            this.SourcePath = new SourcePath(tr);
        }

        public ProtoFormatException(string message, Exception innerException, TokenReader tr)
            : base(message, innerException)
        {
            this.SourcePath = new SourcePath(tr);
        }
    }
}

