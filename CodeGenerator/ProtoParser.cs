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
					
					//Remove comment
					int comment = line.IndexOf ("//");
					if (comment >= 0)
						line = line.Substring (0, comment);
					t += line + "\n";
					
				}
			}
			
			TokenReader tr = new TokenReader (t);
			Exception e = null;
			try {
				ParseMessages (tr, p);
				return p;
			} catch (InvalidDataException ide) {
				e = ide;
			} catch (FormatException fe) {
				e = fe;
			}
			Console.Write (tr.Parsed);
			Console.WriteLine (" <---");
			Console.WriteLine (e.Message);
			Console.WriteLine ("Got: " + tr.Next);
			return null;
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
					if (tr.ReadNext () != ";")
						throw new InvalidDataException ("Expected ;");
					break;
				case "package":
					string pkg = tr.ReadNext ();
					if (tr.ReadNext () != ";")
						throw new InvalidDataException ("Expected ;");
					p.OptionNamespace = pkg;
					break;
				default:
					throw new InvalidDataException ("Unexpected/not implemented: " + token);
				}
			}
		}

		static Message ParseMessage (TokenReader tr, Message parent)
		{
			Message msg = new Message (parent);
			msg.ProtoName = tr.ReadNext ();
			
			//Expect "{"
			if (tr.ReadNext () != "{")
				throw new InvalidDataException ("Expected: {");
			
			while (ParseField (tr, msg))
				continue;
			
			return msg;			
		}

		static bool ParseField (TokenReader tr, Message m)
		{
			string rule = tr.ReadNext ();
			if (rule == "}")
				return false;
			
			if (rule == "enum") {
				MessageEnum me = ParseEnum (tr, m);
				m.Enums.Add (me);
				return true;
			}

			Field f = new Field ();
			
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
				throw new InvalidDataException ("unknown rule: " + rule);
			}

			//Type
			f.ProtoTypeName = tr.ReadNext ();
			
			//Name
			f.Name = tr.ReadNext ();
			
			//ID
			if (tr.ReadNext () != "=")
				throw new InvalidDataException ("Expected: =");
			f.ID = int.Parse (tr.ReadNext ());
			if (19000 <= f.ID && f.ID <= 19999)
				throw new InvalidDataException ("Can't use reserved field ID 19000-19999");
			if (f.ID > 536870911)
				throw new InvalidDataException ("Maximum field id is 2^29 - 1");

			//Add Field to message
			m.Fields.Add (f.ID, f);
			
			//Determine if extra options
			string extra = tr.ReadNext ();
			if (extra == ";")
				return true;
			
			//Field options
			if (extra != "[")
				throw new InvalidDataException ("Expected: [");
			
			while (true) {
				string key = tr.ReadNext ();
				if (tr.ReadNext () != "=")
					throw new InvalidDataException ("Expected: =");
				string val = tr.ReadNext ();
				
				ParseFieldOption (key, val, f);
				string optionSep = tr.ReadNext ();
				if (optionSep == "]")
					break;
				if (optionSep == ",")
					continue;
				throw new InvalidDataException (@"Expected "","" or ""]""");
			}
			if (tr.ReadNext () != ";")
				throw new InvalidDataException ("Expected: ;");
			
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
			case "externaltype":
				f.OptionCustomType = val;
				break;
			case "generate":
				f.OptionGenerate = Boolean.Parse (val);
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
				throw new InvalidDataException ("Expected: =");
			//Read value
			string value = tr.ReadNext ();
			if (tr.ReadNext () != ";")
				throw new InvalidDataException ("Expected: ;");
			
			//null = ignore option
			if (m == null)
				return;
			
			switch (key) {
			case "namespace":
				m.OptionNamespace = value;
				break;
			default:
				Console.WriteLine ("Warning: Unknown option: " + key);
				break;
			}
		}

		static MessageEnum ParseEnum (TokenReader tr, Message parent)
		{
			MessageEnum me = new MessageEnum (parent);
			me.ProtoName = tr.ReadNext ();
			
			if (tr.ReadNext () != "{")
				throw new InvalidDataException ("Expected: {");
			
			while (true) {
				string name = tr.ReadNext ();
				
				if (name == "}")
					return me;
				
				//Ignore options
				if (name == "option") {
					ParseOption (tr, null);
					continue;
				}
				
				if (tr.ReadNext () != "=")
					throw new InvalidDataException ("Expected: =");
				
				int id = int.Parse (tr.ReadNext ());
				
				me.Enums.Add (name, id);
				
				if (tr.ReadNext () != ";")
					throw new InvalidDataException ("Expected: ;");
			}
			
		}
	}
}

