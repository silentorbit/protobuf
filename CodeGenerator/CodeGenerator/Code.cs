using System;

namespace ProtocolBuffers
{
	static class Code
	{	
		/// <summary>
		/// Code.Indent all lines in the code string with one tab
		/// </summary>
		public static string Indent (string code)
		{
			return Code.Indent (1, code);
		}
		
		/// <summary>
		/// Code.Indent all lines in the code string with given number of tabs
		/// </summary>
		public static string Indent (int tabs, string code)
		{
			string sep = "\n";
			for (int n = 0; n < tabs; n++)
				sep += "\t";
			code = sep + string.Join (sep, code.Split ('\n'));
			return code.Substring (1).TrimEnd ('\t');			
		}
		
		public static string Prefix (string prefix, string code)
		{
			string sep = "\n" + prefix;
			code = sep + string.Join (sep, code.Split ('\n'));
			return code.Substring (1);
		}
		
		public static string Comment (string code)
		{
			return "//" + string.Join ("\n//", code.Split ('\n'));
			;
		}
		
		public static string Summary (string summary)
		{
			return "/// <summary>\n/// " + string.Join ("\n/// ", summary.Split ('\n')) + "\n/// </summary>\n";
		}

	}
}

