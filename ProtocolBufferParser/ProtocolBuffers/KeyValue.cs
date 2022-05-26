namespace SilentOrbit.ProtocolBuffers;

/// <summary>
/// Storage of unknown fields
/// </summary>
public class KeyValue
{
    public Key Key { get; set; }

    public byte[] Value { get; set; }

    public KeyValue(Key key, byte[] value)
    {
        this.Key = key;
        this.Value = value;
    }

    public override string ToString()
    {
        return string.Format("[KeyValue: {0}, {1}, {2} bytes]", Key.Field, Key.WireType, Value.Length);
    }
}
