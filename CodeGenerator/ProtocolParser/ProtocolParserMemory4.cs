using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// MemoryStream management.
/// .NET 4 features not added when the --net3 flag is being used
/// </summary>
namespace SilentOrbit.ProtocolBuffers
{
    using System.Collections.Concurrent;

    public class ConcurrentBagStack : MemoryStreamStack
    {
        ConcurrentBag<MemoryStream> bag = new ConcurrentBag<MemoryStream>();

        /// <summary>
        /// The returned stream is not reset.
        /// You must call .SetLength(0) before using it.
        /// This is done in the generated code.
        /// </summary>
        public MemoryStream Pop()
        {
            MemoryStream result;

            if (bag.TryTake(out result))
                return result;
            else
                return new MemoryStream();
        }

        public void Push(MemoryStream stream)
        {
            bag.Add(stream);
        }

        public void Dispose()
        {
            throw new InvalidOperationException("ConcurrentBagStack.Dispose() should not be called.");
        }
    }
}

