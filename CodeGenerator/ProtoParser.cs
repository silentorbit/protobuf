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
				default:
					throw new InvalidDataException ("Expected: message or option");
				}
			}
		}

		static Message ParseMessage (TokenReader tr, Message parent)
		{
			Message msg = new Message (parent);
			msg.ProtoName = tr.ReadNext ();
			
			//Expect "{"
			if (tr.ReadNext () != "{")
				throw new InvalidDataException ("Ecpected: {");
			
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
				f.Rule = Rules.Required;
				break;
			case "optional":
				f.Rule = Rules.Optional;
				break;
			case "repeated":
				f.Rule = Rules.Repeated;
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
			m.Fields.Add (f);

			//Type
			f.ProtoTypeName = tr.ReadNext ();
			
			//Name
			f.Name = tr.ReadNext ();
			
			//ID
			if (tr.ReadNext () != "=")
				throw new InvalidDataException ("Expected: =");
			f.ID = uint.Parse (tr.ReadNext ());
			if (19000 <= f.ID && f.ID <= 19999)
				throw new InvalidDataException ("Can't use reserved field ID 19000-19999");
			if (f.ID > 536870911)
				throw new InvalidDataException ("Maximum field id is 2^29 - 1");
			
			//Determine if extra options
			string extra = tr.ReadNext ();
			if (extra == ";")
				return true;
			
			//Field options
			if (extra != "[")
				throw new InvalidDataException ("Expected: [");
			
			while (true) {
				string option = tr.ReadNext ();
				if (tr.ReadNext () != "=")
					throw new InvalidDataException ("Expected: =");
				string value = tr.ReadNext ();
				
				switch (option) {
				case "default":
					f.Default = value;
					break;
				case "packed":
					f.Packed = Boolean.Parse (value);
					break;
				case "deprecated":
					f.Deprecated = Boolean.Parse (value);
					break;
				default:
					//Ignore unknown options
					break;
				}
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
			
			if (m != null)
				m.Options.Add (key, value);
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

