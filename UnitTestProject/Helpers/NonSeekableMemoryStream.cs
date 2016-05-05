using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Helpers
{
    /// <summary>
    /// MemoryStream With Seek() removed.
    /// Used to test PositionStream that will add this feature back.
    /// </summary>
    class NonSeekableMemoryStream : MemoryStream
    {
        public NonSeekableMemoryStream(byte[] buffer) : base(buffer)
        {
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException();
        }
    }
}
