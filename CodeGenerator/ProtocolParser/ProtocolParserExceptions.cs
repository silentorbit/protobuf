//
// Exception used in the generated code
//
using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    ///<summary>>
    /// This exception is thrown when badly formatted protocol buffer data is read.
    ///</summary>
    public class ProtocolBufferException : Exception
    {
        public ProtocolBufferException(string message) : base(message)
        {
        }
    }
}

