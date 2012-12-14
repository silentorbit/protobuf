using System;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// messages and fields carrying comments
    /// </summary>
    public interface IComment
    {
        string Comments { get; set; }
    }
}

