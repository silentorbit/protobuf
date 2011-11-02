using System;

namespace ProtocolBuffers
{
	public class MessageName : Message
	{
		public MessageName (Message parent, string fullNamespaceClass)
			: base(parent)
		{
			int pos = fullNamespaceClass.LastIndexOf (".");
			if (pos < 0) {
				this.CSType = fullNamespaceClass;
			} else {
				this.OptionNamespace = fullNamespaceClass.Substring (0, pos);
				this.CSType = fullNamespaceClass.Substring (pos + 1);
			}
		}
	}
}

