using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
	public class MessageEnum : IProtoType
	{
		public Message Parent { get; set; }
	
		public string ProtoName { get; set; }
		
		public string CSName { get; set; }

		public Dictionary<string,int> Enums { get; set; }
		
		#region Local options
		
		public string OptionNamespace { get; set; }
		
		#endregion

		public MessageEnum (Message parent)
		{
			this.Parent = parent;
			this.Enums = new Dictionary<string, int> ();
		}
	}
}

