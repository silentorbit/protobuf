using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	public class MessageEnum : MessageEnumBase
	{
		public Dictionary<string,int> Enums = new Dictionary<string, int>();
		
		public MessageEnum (Message parent)
		{
			this.Parent = parent;
		}
	}
}

