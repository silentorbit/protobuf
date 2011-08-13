using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	public class Message : IProtoType
	{
		public Message Parent { get; set; }

		public string ProtoName { get; set; }

		public string CSName { get; set; }

		public List<Field> Fields { get; set; }

		public List<Message> Messages { get; set; }

		public List<MessageEnum> Enums { get; set; }

		#region Local options
		
		public virtual string Namespace {
			get {
				if (OptionNamespace == null)
					return Parent.Namespace;
				else 
					return OptionNamespace;
			}
		}
		
		public string OptionNamespace { get; set; }

		public enum TriggerOptions
		{
			/// <summary>
			/// Default. Use and generate template code.
			/// </summary>
			Generate,
			/// <summary>
			/// Use, but assume implemented elsewhere.
			/// </summary>
			Yes,
			/// <summary>
			/// Dont use
			/// </summary>
			No,			
		}
		/// <summary>
		/// Whether to use triggers before/after serialization and to generate templates.
		/// </summary>
		public TriggerOptions OptionTriggers { get; set; }
		
		#endregion
		
		public Message (Message parent)
		{
			this.Parent = parent;
			this.Fields = new List<Field> ();
			this.Messages = new List<Message> ();
			this.Enums = new List<MessageEnum> ();
			
			this.OptionNamespace = null;
			this.OptionTriggers = TriggerOptions.Generate;
		}
		
		public override string ToString ()
		{
			return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
		}
	}
}

