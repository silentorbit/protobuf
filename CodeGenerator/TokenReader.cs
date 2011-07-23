using System;
using System.IO;

namespace ProtocolBuffers
{
	public class TokenReader
	{
		readonly string whitespace = " \t\r\n";
		readonly string singletoken = "{}=[];";
		readonly string text;
		
		public TokenReader (string text)
		{
			this.text = text;			
		}
		
		public string Parsed {
			get {
				return text.Substring (0, offset);
			}
		}
		
		public string Next {
			get {
				return text.Substring (offset, 1);
			}
		}
		
		int offset;

		private string GetChar ()
		{
			if (offset >= text.Length)
				throw new EndOfStreamException ();
			
			char c = text [offset];
			offset += 1;
			return c.ToString ();
		}
		
		public string ReadNext ()
		{
			string c;	//Character
			
			//Skip whitespace characters
			while (true) {
				c = GetChar ();
				if (whitespace.Contains (c))
					continue;
				break;
			}
			
			//Determine token type
			if (singletoken.Contains (c))
				return c.ToString ();
		
			//Follow token
			string token = c;
			
			while (true) {
				c = GetChar ();
				if (whitespace.Contains (c) || singletoken.Contains (c)) {
					offset -= 1;
					return token;
				}
				
				token += c;
			}
			
		}
	}
}

