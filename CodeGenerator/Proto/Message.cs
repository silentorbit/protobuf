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
				/// (C#) access modifier: public(default)/protected/private
				/// </summary>
				public string OptionAccess  { get; set; }
		
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
				/// Don't create class/struct, useful when serializing types in external DLL
				/// </summary>
				public bool OptionExternal { get; set; }

				/// <summary>
				/// Make message a struct rather than a class
				/// </summary>
				public bool OptionStruct { get; set; }

		#endregion
		
				public Message (Message parent)
				{
						this.Parent = parent;
		
						this.OptionNamespace = null;
						this.OptionAccess = "public";
						this.OptionTriggers = false;
						this.OptionPreserveUnknown = false;
						this.OptionExternal = false;
						this.OptionStruct = false;
				}
		
				public override string ToString ()
				{
						return string.Format ("[Message: Name={0}, Fields={1}, Enums={2}]", ProtoName, Fields.Count, Enums.Count);
				}
		
		}
}

