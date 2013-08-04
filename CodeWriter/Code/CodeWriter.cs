using System;
using System.Text;
using System.IO;

namespace SilentOrbit.Code
{
    /// <summary>
    /// Static and instance helpers for code generation
    /// </summary>
    public class CodeWriter : IDisposable
    {
        #region Settings
        public static string DefaultIndentPrefix = "    ";
        public static string DefaultNewLine = "\r\n";

        public string IndentPrefix;
        public string NewLine;
        #endregion

        #region Constructors
        readonly TextWriter w;
        MemoryStream ms = new MemoryStream();

        /// <summary>
        /// Writes to memory, get the code using the "Code" property
        /// </summary>
        public CodeWriter()
        {
            this.IndentPrefix = DefaultIndentPrefix;
            this.NewLine = DefaultNewLine;

            ms = new MemoryStream();
            w = new StreamWriter(ms, Encoding.UTF8);
            //w.NewLine = NewLine; // does not appear to work
        }

        /// <summary>
        /// Writes code directly to file
        /// </summary>
        public CodeWriter(string csPath)
        {
            this.IndentPrefix = DefaultIndentPrefix;
            this.NewLine = DefaultNewLine;

            w = new StreamWriter(csPath, false, Encoding.UTF8);
        }

        /// <summary>
        /// Return the generated code as a string.
        /// </summary>
        public string Code
        {
            get
            {
                w.Flush();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public virtual void Flush()
        {
            w.Flush();
        }

        public virtual void Dispose()
        {
            w.Close();
        }
        #endregion

        #region Indentation
        /// <summary>
        /// Level of indentation
        /// </summary>
        public int IndentLevel { get; private set; }

        /// <summary>
        /// Accumulated prefixes over indentations
        /// </summary>
        string prefix = "";

        public void Indent()
        {
            IndentLevel += 1;
            prefix += IndentPrefix;
        }

        public void Dedent()
        {
            IndentLevel -= 1;
            if (IndentLevel < 0)
                throw new InvalidOperationException("Indent error");
            prefix = prefix.Substring(0, prefix.Length - IndentPrefix.Length);
        }
        #endregion

        public void Attribute(string attributeConstructor)
        {
            WriteLine("[" + attributeConstructor + "]");
        }

        /// <summary>
        /// Write leading bracket and indent
        /// </summary>
        /// <param name="str">String.</param>
        public void Bracket()
        {
            WriteLine("{");
            Indent();
        }

        /// <summary>
        /// Write leading bracket and indent
        /// </summary>
        /// <param name="str">Line before bracket</param>
        public void Bracket(string str)
        {
            WriteLine(str);
            WriteLine("{");
            Indent();
        }

        public void Using(string str)
        {
            WriteLine("using (" + str + ")");
            WriteLine("{");
            Indent();
        }

        public void IfBracket(string str)
        {
            WriteLine("if (" + str + ")");
            WriteLine("{");
            Indent();
        }

        public void WhileBracket(string str)
        {
            WriteLine("while (" + str + ")");
            WriteLine("{");
            Indent();
        }

        public void Switch(string str)
        {
            Bracket("switch (" + str + ")");
            Indent();
        }

        public void SwitchEnd()
        {
            Dedent();
            EndBracket();
        }

        public void Case(string str)
        {
            Dedent();
            WriteLine("case " + str + ":");
            Indent();
        }

        public void Case(int id)
        {
            Dedent();
            WriteLine("case " + id + ":");
            Indent();
        }

        public void CaseDefault()
        {
            Dedent();
            WriteLine("default:");
            Indent();
        }

        public void ForeachBracket(string str)
        {
            WriteLine("foreach (" + str + ")");
            WriteLine("{");
            Indent();
        }

        public void EndBracket()
        {
            Dedent();
            WriteLine("}");
        }

        public void EndBracketSpace()
        {
            Dedent();
            WriteLine("}");
            WriteLine();
        }

        /// <summary>
        /// Writes a singe line indented.
        /// </summary>
        public void WriteIndent(string str)
        {
            WriteLine(IndentPrefix + str);
        }

        public void WriteLine(string line)
        {
            foreach (string l in SplitTrimEnd(line))
            {
                string pl = (prefix + l).TrimEnd(' ', '\t');
                w.Write(pl + NewLine);
            }
        }

        public void WriteLine()
        {
            WriteLine("");
        }

        #region Comments
        public void Comment(string code)
        {
            if (code == null)
                return;

            const string commentPrefix = "// ";
            prefix += commentPrefix;
            foreach (string line in SplitTrimEnd(code))
                WriteLine(line);
            prefix = prefix.Substring(0, prefix.Length - commentPrefix.Length);
        }

        public void Summary(string summary)
        {
            if (summary == null || summary.Trim() == "")
                return;

            string[] lines = SplitTrimEnd(summary);
            if (lines.Length == 1)
            {
                WriteLine("/// <summary>" + lines[0] + "</summary>");
                return;
            }

            prefix += "/// ";
            WriteLine("<summary>");
            foreach (string line in lines)
                WriteLine("<para>" + line + "</para>");
            WriteLine("</summary>");
            prefix = prefix.Substring(0, prefix.Length - 4);
        }

        #endregion

        /// <summary>
        /// Split string into an array of lines and trim whitespace at the end
        /// </summary>
        static string[] SplitTrimEnd(string text)
        {
            var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int n = 0; n < lines.Length; n++)
                lines[n] = lines[n].TrimEnd(' ', '\t');
            return lines;
        }
    }
}

