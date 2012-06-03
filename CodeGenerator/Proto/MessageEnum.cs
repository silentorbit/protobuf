using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	class MessageEnum : MessageEnumBase
	{
		public string Comments;
		public Dictionary<string,int> Enums = new Dictionary<string, int> ();
		public Dictionary<string,string> EnumsComments = new Dictionary<string, string> ();
		
		public MessageEnum (Message parent)
		{
			this.Parent = parent;
		}
	}
}

