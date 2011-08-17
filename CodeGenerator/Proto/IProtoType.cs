using System;

namespace ProtocolBuffers
{
	public interface IProtoType
	{
		Message Parent { get; set; }

		string CSName { get; set; }
		
		#region local options
		
		/// <summary>
		/// Helper to get the full namespace
		/// </summary>
		string Namespace { get; }

		string OptionNamespace { get; set; }
		
		#endregion
	}
}

