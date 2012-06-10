using System;
using System.IO;
using System.Text;

namespace ProtocolBuffers
{
    /// <summary>
    /// Helper for CsProtoParser.
    /// Skip comments and empty lines.
    /// </summary>
    class CsProtoReader : IDisposable
    {
        readonly string path;
        readonly TextReader stream;

        public int Line { get; private set; }

        public CsProtoReader(string path)
        {
            this.path = path;
            stream = new StreamReader(path, Encoding.UTF8);
        }

        /// <summary>
        /// Return next non empty line, including comments
        /// </summary>
        public string ReadLine()
        {
            while (true)
            {
                string line = stream.ReadLine();
                if (line == null)
                    return null;
                Line += 1;

                line = RemoveComments(line);
                if(line.Trim() == "")
                    continue;
                line = line.TrimEnd(' ', '\t');
                return line;
            }
        }

        string RemoveComments(string line)
        {
            int pos = line.IndexOf("//");
            if(pos < 0)
                return line;

            line = line.Substring(0, pos);
            return line;
        }

        public void Dispose ()
        {
            stream.Dispose();
        }

        public override string ToString()
        {
            return "Line " + Line + " @ " + path;
        }
    }
}

