namespace SilentOrbit.ProtocolBuffers.Parsers;

public class SourcePath
{
    public readonly string Path;
    public readonly int Line;
    public readonly int Column;

    public SourcePath(TokenReader tr)
    {
        Path = System.IO.Path.GetFullPath(tr.Path);
        Line = tr.Parsed.Split('\n').Length - 1;
        Column = tr.Parsed.Length - tr.Parsed.LastIndexOf('\n') + 1;
    }
}
