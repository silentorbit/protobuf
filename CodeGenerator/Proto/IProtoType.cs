using System;

namespace ProtocolBuffers
{
	public interface IProtoType
	{
		Message Parent { get; set; }

		string CSName { get; set; }
		
		#region local options
		
		string OptionNamespace { get; set; }
		
		#endregion
	}
}

