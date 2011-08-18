using System;

namespace ProtocolBuffers
{
	public abstract class MessageEnumBase
	{
		public Message Parent { get; set; }
		
		public string ProtoName { get; set; }
		
		/// <summary>
		/// The c# type name
		/// </summary>
		public string CSType { get; set; }

		#region local options
		
		public virtual string Namespace {
			get {
				if (OptionNamespace == null) {
					if (Parent is Proto)
						return Parent.Namespace;
					else
						return Parent.Namespace + "." + Parent.CSType;
				} else 
					return OptionNamespace;
			}
		}
		
		public string OptionNamespace { get; set; }
		
		#endregion
		
		public string FullCSType {
			get {
				string path = CSType;
				MessageEnumBase message = this;
				while (message.Parent != null && !(message.Parent is Proto)) {
					message = message.Parent;
					path = message.CSType + "." + path;
				}
				return message.Namespace + "." + path;
			}
		}

	}
}

