using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	
	/// <summary>
	/// Representation of a .proto file
	/// </summary>
	public class Proto : Message
	{
		public Proto () : base(null)
		{
		}
		
		/// <summary>
		/// Defaults to Example if not specified
		/// </summary>
		public override string Namespace {
			get {
				if (OptionNamespace == null)
					return "Example";
				return OptionNamespace;
			}
		}
		
		public override string ToString ()
		{
			string t = "Proto: ";
			foreach (Message m in Messages)
				t += "\n\t" + m;
			return t;
		}
	}
	
}

