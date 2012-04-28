using System;
using System.IO;
using System.Text;

namespace ProtocolBuffers
{
	public static class ProtoParser
	{
		public static Proto Parse (string path)
		{
			Proto p = new Proto ();
			
			string t = "";
			
			using (TextReader reader = new StreamReader(path, Encoding.UTF8)) {
				while (true) {
					string line = reader.ReadLine ();
					if (line == null)
						break;
					
					t += line + "\n";
				}
			}
			
			TokenReader tr = new TokenReader (t);
			ProtoFormatException e = null;
			try {
				ParseMessages (tr, p);
				return p;
			} catch (ProtoFormatException pfe) {
				e = pfe;
			}
			Console.Write (tr.Parsed);
			Console.WriteLine (" <---");
			Console.WriteLine (e.Message);
			return null;
		}

		static string lastComment = null;
		
		/// <summary>
		/// Return true if token was a comment
		/// </summary>
		static bool ParseComment (string token)
		{
			if (token.StartsWith ("//") == false)
				return false;
			token = token.TrimStart ('/');
			if (lastComment == null)
				lastComment = token;
			else
				lastComment += "\r\n" + token;
			return true;		
		}
		
		static void ParseMessages (TokenReader tr, Proto p)
		{
			while (true) {
				string token;
				try {
					token = tr.ReadNext ();
				} catch (EndOfStreamException) {
					return;
				}
				if (ParseComment (token))
					continue;
				
				switch (token) {
				case "message":
					p.Messages.Add (ParseMessage (tr, p));
					break;
				case "option":
					//Save options
					ParseOption (tr, p);
					break;
				case "import": //Ignored
					tr.ReadNext ();
					tr.ReadNextOrThrow (";");
					break;
				case "package":
					string pkg = tr.ReadNext ();
					tr.ReadNextOrThrow (";");
					p.OptionNamespace = pkg;
					break;
				default:
					throw new ProtoFormatException ("Unexpected/not implemented: " + token);
				}
			}
		}

		static Message ParseMessage (TokenReader tr, Message parent)
		{
			Message msg = new Message (parent);
			msg.Comments = lastComment;
			lastComment = null;
			msg.ProtoName = tr.ReadNext ();
			
			tr.ReadNextOrThrow ("{");
			
			while (ParseField (tr, msg))
				continue;
			
			return msg;			
		}

		static bool ParseField (TokenReader tr, Message m)
		{
			string rule = tr.ReadNext ();
			while (true) {
				if (ParseComment (rule) == false)
					break;
				rule = tr.ReadNext ();
			}

			if (rule == "}")
				return false;
			
			if (rule == "enum") {
				MessageEnum me = ParseEnum (tr, m);
				m.Enums.Add (me);
				return true;
			}

			Field f = new Field ();
			f.Comments = lastComment;
			lastComment = null;
			
			//Rule
			switch (rule) {
			case "required":
				f.Rule = FieldRule.Required;
				break;
			case "optional":
				f.Rule = FieldRule.Optional;
				break;
			case "repeated":
				f.Rule = FieldRule.Repeated;
				break;
			case "option":
				//Save options
				ParseOption (tr, m);
				return true;
			case "message":
				m.Messages.Add (ParseMessage (tr, m));
				return true;
			default:
				throw new ProtoFormatException ("unknown rule: " + rule);
			}

			//Type
			f.ProtoTypeName = tr.ReadNext ();
			
			//Name
			f.Name = tr.ReadNext ();
			
			//ID
			tr.ReadNextOrThrow ("=");
			f.ID = int.Parse (tr.ReadNext ());
			if (19000 <= f.ID && f.ID <= 19999)
				throw new ProtoFormatException ("Can't use reserved field ID 19000-19999");
			if (f.ID > (1 << 29) - 1)
				throw new ProtoFormatException ("Maximum field id is 2^29 - 1");

			//Add Field to message
			m.Fields.Add (f.ID, f);
			
			//Determine if extra options
			string extra = tr.ReadNext ();
			if (extra == ";")
				return true;
			
			//Field options
			if (extra != "[")
				throw new ProtoFormatException ("Expected: [ got " + extra);
			
			while (true) {
				string key = tr.ReadNext ();
				tr.ReadNextOrThrow ("=");
				string val = tr.ReadNext ();
				
				ParseFieldOption (key, val, f);
				string optionSep = tr.ReadNext ();
				if (optionSep == "]")
					break;
				if (optionSep == ",")
					continue;
				throw new ProtoFormatException (@"Expected "","" or ""]"" got " + tr.Next);
			}
			tr.ReadNextOrThrow (";");
			
			return true;
		}

		static void ParseFieldOption (string key, string val, Field f)
		{
			switch (key) {
			case "default":
				f.OptionDefault = val;
				break;
			case "packed":
				f.OptionPacked = Boolean.Parse (val);
				break;
			case "deprecated":
				f.OptionDeprecated = Boolean.Parse (val);
				break;
				
			//Local options:
				
			case "access":
				f.OptionAccess = val;
				break;
			case "codetype":
				if (val == "DateTime" || val == "TimeSpan") {
					if (f.ProtoTypeName != "int64")
						throw new ProtoFormatException ("DateTime and TimeSpan must be stored in int64. was " + f.ProtoTypeName);
				}
				f.OptionCodeType = val;
				break;
			case "generate":
				f.OptionGenerate = Boolean.Parse (val);
				break;
			case "readonly":
				f.OptionReadOnly = Boolean.Parse (val);
				break;
			default:
				Console.WriteLine ("Warning: Unknown field option: " + key);
				break;
			}
		}
		
		/// <summary>
		/// File or Message options
		/// </summary>
		static void ParseOption (TokenReader tr, Message m)
		{
			//Read name
			string key = tr.ReadNext ();
			if (tr.ReadNext () != "=")
				throw new ProtoFormatException ("Expected: = got " + tr.Next);
			//Read value
			string value = tr.ReadNext ();
			if (tr.ReadNext () != ";")
				throw new ProtoFormatException ("Expected: ; got " + tr.Next);
			
			//null = ignore option
			if (m == null)
				return;
			
			switch (key) {
			case "namespace":
				m.OptionNamespace = value;
				break;
			case "triggers":
				m.OptionTriggers = Boolean.Parse (value);
				break;
			case "preserveunknown":
				m.OptionPreserveUnknown = Boolean.Parse (value);
				break;
			case "access":
				m.OptionAccess = value;
				break;
			default:
				Console.WriteLine ("Warning: Unknown option: " + key);
				break;
			}
		}

		static MessageEnum ParseEnum (TokenReader tr, Message parent)
		{
			MessageEnum me = new MessageEnum (parent);
			me.Comments = lastComment;
			lastComment = null;
			me.ProtoName = tr.ReadNext ();
			
			if (tr.ReadNext () != "{")
				throw new ProtoFormatException ("Expected: {");
			
			while (true) {
				string name = tr.ReadNext ();
				
				if (ParseComment (name))
					continue;
				
				if (name == "}")
					return me;
				
				//Ignore options
				if (name == "option") {
					ParseOption (tr, null);
					lastComment = null;
					continue;
				}
				
				if (tr.ReadNext () != "=")
					throw new ProtoFormatException ("Expected: =");
				
				int id = int.Parse (tr.ReadNext ());
				
				me.Enums.Add (name, id);
				if (lastComment != null)
					me.EnumsComments.Add (name, lastComment);
				lastComment = null;
				
				if (tr.ReadNext () != ";")
					throw new ProtoFormatException ("Expected: ;");
			}
			
		}
	}
}

