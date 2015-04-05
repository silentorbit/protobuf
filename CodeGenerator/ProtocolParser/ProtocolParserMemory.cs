using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// MemoryStream management
/// </summary>
namespace SilentOrbit.ProtocolBuffers
{
    public interface MemoryStreamStack : IDisposable
    {
        MemoryStream Pop();

        void Push(MemoryStream stream);
    }

    /// <summary>
    /// Thread safe stack of memory streams
    /// </summary>
    public class ThreadSafeStack : MemoryStreamStack
    {
        Stack<MemoryStream> stack = new Stack<MemoryStream>();

        /// <summary>
        /// The returned stream is not reset.
        /// You must call .SetLength(0) before using it.
        /// This is done in the generated code.
        /// </summary>
        public MemoryStream Pop()
        {
            lock (stack)
            {
                if (stack.Count == 0)
                    return new MemoryStream();
                else
                    return stack.Pop();
            }
        }

        public void Push(MemoryStream stream)
        {
            lock (stack)
            {
                stack.Push(stream);
            }
        }

        public void Dispose()
        {
            lock (stack)
            {
                stack.Clear();
            }
        }
    }

    /// <summary>
    /// Non-thread safe stack of memory streams
    /// Safe as long as only one thread is Serializing
    /// </summary>
    public class ThreadUnsafeStack : MemoryStreamStack
    {
        Stack<MemoryStream> stack = new Stack<MemoryStream>();

        /// <summary>
        /// The returned stream is not reset.
        /// You must call .SetLength(0) before using it.
        /// This is done in the generated code.
        /// </summary>
        public MemoryStream Pop()
        {
            if (stack.Count == 0)
                return new MemoryStream();
            else
                return stack.Pop();
        }

        public void Push(MemoryStream stream)
        {
            stack.Push(stream);
        }

        public void Dispose()
        {
            stack.Clear();
        }
    }

    /// <summary>
    /// Unoptimized stack, allocates a new MemoryStream for every request.
    /// </summary>
    public class AllocationStack : MemoryStreamStack
    {
        /// <summary>
        /// The returned stream is not reset.
        /// You must call .SetLength(0) before using it.
        /// This is done in the generated code.
        /// </summary>
        public MemoryStream Pop()
        {
            return new MemoryStream();
        }

        public void Push(MemoryStream stream)
        {
            //No need to Dispose MemoryStream
        }

        public void Dispose()
        {
        }
    }

    public static partial class ProtocolParser
    {
        /// <summary>
        /// Experimental stack of MemoryStream
        /// </summary>
        public static MemoryStreamStack Stack = new AllocationStack();
    }
}

