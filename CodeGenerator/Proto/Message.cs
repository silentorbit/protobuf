using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	public class Message : MessageEnumBase
	{
		public Dictionary<int, Field> Fields = new  Dictionary<int, Field> ();
		public List<Message> Messages = new List<Message> ();
		public List<MessageEnum> Enums = new List<MessageEnum> ();

		#region Local options
		
		/// <summary>
		/// Call triggers before/after serialization.
		/// </summary>
		public bool OptionTriggers { get; set; }
		
		#endregion
		
		public Message (Message parent)
		{
			this.Parent = parent;
			
			this.OptionNamespace = null;
			this.OptionTriggers = false;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
		}
		
	}
}

