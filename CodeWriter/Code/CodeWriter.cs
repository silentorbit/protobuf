using System;
using System.Text;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// Static and instance helpers for code generation
    /// </summary>
	public class CodeWriter : IDisposable
	{ 
        #region Settings
		public static string IndentPrefix = "    ";
        #endregion

        #region Constructors

		readonly TextWriter w;
		MemoryStream ms = new MemoryStream();
        /// <summary>
        /// Writes to memory, get the code using the "Code" property
        /// </summary>
		public CodeWriter()
		{
			ms = new MemoryStream();
			w = new StreamWriter(ms, Encoding.UTF8);
			//w.NewLine = "\r\n"; // does not appear to work
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
        /// <summary>
        /// Writes code directly to file
        /// </summary>
		public CodeWriter(string csPath)
		{
			w = new StreamWriter(csPath, false, Encoding.UTF8);
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
			string[] lines = line.Split('\n');
			foreach (string l in lines)
			{
				w.Write(prefix + l + "\r\n");
			}
		}

		public void WriteLine()
		{
			WriteLine(prefix.TrimEnd(' '));
		}
        #region Comments

		public void Comment(string code)
		{
			if (code == null)
				return;

			prefix += "// ";
			foreach (string line in code.Split('\n'))
				WriteLine(line.TrimEnd(' '));
			prefix = prefix.Substring(0, prefix.Length - 3);
		}

		public void Summary(string summary)
		{
			if (summary == null || summary.Trim() == "")
				return;

			string[] lines = summary.Replace("\r\n", "\n").Split('\n');
			if (lines.Length == 1)
			{
				WriteLine("/// <summary>" + summary.TrimEnd(' ') + "</summary>");
				return;
			}

			prefix += "/// ";
			WriteLine("<summary>");
			foreach (string line in lines)
				WriteLine("<para>" + line.TrimEnd(' ') + "</para>");
			WriteLine("</summary>");
			prefix = prefix.Substring(0, prefix.Length - 4);
		}
        #endregion
	}
}

