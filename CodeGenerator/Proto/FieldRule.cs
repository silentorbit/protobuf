using System;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// Rules for fields in .proto files
    /// </summary>
    enum FieldRule
    {
        Required,   //a well-formed message must have exactly one of this field.
        Optional,   //a well-formed message can have zero or one of this field (but not more than one).
        Repeated,   //this field can be repeated any number of times (including zero) in a well-formed message. The order of the repeated values will be preserved.
    }
}

