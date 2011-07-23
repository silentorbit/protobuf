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
				if (token == "message")
					ParseMessage (tr, p);
				else
					throw new InvalidDataException ("Expected: message");
			}
		}

		static void ParseMessage (TokenReader tr, Proto p)
		{
			Message msg = new Message ();
			msg.Name = tr.ReadNext ();
			
			//Expect "{"
			if (tr.ReadNext () != "{")
				throw new InvalidDataException ("Ecpected: {");
			
			while (ParseField (tr, msg))
				continue;
			
			p.Messages.Add (msg.Name, msg);
		}

		static bool ParseField (TokenReader tr, Message m)
		{
			string rule = tr.ReadNext ();
			if (rule == "}")
				return false;
			
			if (rule == "enum") {
				MessageEnum me = ParseEnum (tr);
				m.Enums.Add (me.Name, me);
				return true;
			}

			Field f = new Field ();
			m.Fields.Add (f);
			
			//Rule
			if (rule == "required")
				f.Rule = Rules.Required;
			else if (rule == "optional")
				f.Rule = Rules.Optional;
			else if (rule == "repeated")
				f.Rule = Rules.Repeated;
			else
				throw new InvalidDataException ("unknown rule: " + rule);
			
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
					throw new NotImplementedException ("Unknown field option: " + option);
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

		static MessageEnum ParseEnum (TokenReader tr)
		{
			MessageEnum me = new MessageEnum ();
			me.Name = tr.ReadNext ();
			
			if (tr.ReadNext () != "{")
				throw new InvalidDataException ("Expected: {");
			
			while (true) {
				string name = tr.ReadNext ();
				
				if (name == "}")
					return me;
				
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

