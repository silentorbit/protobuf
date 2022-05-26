namespace SilentOrbit.ProtocolBuffers;

public enum Wire
{
    /// <summary>
    /// int32, int64, UInt32, UInt64, SInt32, SInt64, bool, enum
    /// </summary>
    Varint = 0,
    
    /// <summary>
    /// fixed64, sfixed64, double
    /// </summary>
    Fixed64 = 1,
    
    /// <summary>
    /// string, bytes, embedded messages, packed repeated fields
    /// </summary>
    LengthDelimited = 2,
    
    /// <summary>
    /// groups
    /// </summary>
    [Obsolete]
    Start = 3,
    
    /// <summary>
    /// groups
    /// </summary>
    [Obsolete]
    End = 4,           //  groups (deprecated)

    /// <summary>
    /// 32-bit, fixed32, SFixed32, float
    /// </summary>
    Fixed32 = 5,
    
    //Max = 7
}
