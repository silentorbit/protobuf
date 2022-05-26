namespace SilentOrbit.ProtocolBuffers;

/// <summary>
/// Representation of the build in data types
/// </summary>
class ProtoBuiltin : ProtoType
{
    #region Const of build in proto types
    public const string Double = "double";
    public const string Float = "float";
    public const string Int32 = "int32";
    public const string Int64 = "int64";
    public const string UInt32 = "uint32";
    public const string UInt64 = "uint64";
    public const string SInt32 = "sint32";
    public const string SInt64 = "sint64";
    public const string Fixed32 = "fixed32";
    public const string Fixed64 = "fixed64";
    public const string SFixed32 = "sfixed32";
    public const string SFixed64 = "sfixed64";
    public const string Bool = "bool";
    public const string String = "string";
    public const string Bytes = "bytes";
    #endregion Const of build in proto types

    public ProtoBuiltin(string name, Wire wire, string csType)
    {
        ProtoName = name;
        WireType = wire;
        base.CsType = csType;
    }

    public override string CsType
    {
        get { return base.CsType; }
        set { throw new InvalidOperationException(); }
    }

    public override string CsNamespace
    {
        get { throw new InvalidOperationException(); }
    }

    public override string FullCsType
    {
        get { return CsType; }
    }

    public override Wire WireType { get; }

    public override int WireSize
    {
        get
        {
            if (ProtoName == ProtoBuiltin.Bool)
            {
                return 1;
            }

            return base.WireSize;
        }
    }
}
