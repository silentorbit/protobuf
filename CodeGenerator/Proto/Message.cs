using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	class Message : MessageEnumBase
	{
		public string Comments;
		public Dictionary<int, Field> Fields = new  Dictionary<int, Field> ();
		public List<Message> Messages = new List<Message> ();
		public List<MessageEnum> Enums = new List<MessageEnum> ();
		
		#region Local options
		
		/// <summary>
		/// Call triggers before/after serialization.
		/// </summary>
		public bool OptionTriggers { get; set; }

		/// <summary>
		/// Keep unknown fields when deserializing and send them back when serializing.
		/// This will generate field to store any unknown keys and their value.
		/// </summary>
		public bool OptionPreserveUnknown { get; set; }

		/// <summary>
		/// (C#) access modifier: public(default)/protected/private
		/// </summary>
		public string OptionAccess {
			get {
				if (optionAccess != null)
					return optionAccess;
				else
					return "public";
			}
			set{ optionAccess = value;}
		}

		private string optionAccess = null;
		
		#endregion
		
		public Message (Message parent)
		{
			this.Parent = parent;
			
			this.OptionNamespace = null;
			this.OptionTriggers = false;
			this.OptionPreserveUnknown = false;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
		}
		
	}
}

