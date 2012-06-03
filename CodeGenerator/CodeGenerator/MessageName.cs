using System;

namespace ProtocolBuffers
{
	class MessageName : Message
	{
		string path;
		
		public MessageName (Message parent, string fullNamespaceClass)
			: base(parent)
		{
			path = fullNamespaceClass;
			int pos = fullNamespaceClass.LastIndexOf (".");
			if (pos < 0) {
				this.CSType = fullNamespaceClass;
			} else {
				this.OptionNamespace = fullNamespaceClass.Substring (0, pos);
				this.CSType = fullNamespaceClass.Substring (pos + 1);
			}
		}
		
		public override string FullCSType {
			get {
				return path;
			}
		}
	}
}

