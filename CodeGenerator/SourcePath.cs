using System;

namespace ProtocolBuffers
{
    internal class SourcePath
    {
        public readonly string Path;
        public readonly int Line;
        public readonly int Column;
        
        public SourcePath(TokenReader tr)
        {
            this.Path = System.IO.Path.GetFullPath(tr.Path);
            this.Line = tr.Parsed.Split('\n').Length - 1;
            this.Column = tr.Parsed.Length - tr.Parsed.LastIndexOf('\n') + 1;
        }
        
    }
}

