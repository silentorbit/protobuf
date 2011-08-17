using System;

namespace ProtocolBuffers
{
	public class MessageInterfaceCode : MessageCode
	{
		public override string GenerateClass (Message m)
		{
			string code = "";

			code += GenerateInterface (m);
			code += "\n";

			//Default class
			code += "public partial class " + m.CSName + " : I" + m.CSName + "\n";
			code += "{\n";
			string enums = GenerateEnums (m);
			if (enums.Length > 0) {
				code += Code.Indent (enums);
				code += "\n";
			}
			code += Code.Indent (GenerateProperties (m));
			code += "\n";
			
			foreach (Message sub in m.Messages) {
				code += "\n";
				code += Code.Indent (GenerateClass (sub));
			}
			code += "}\n";
			return code;
		}
		
		private string GenerateInterface (Message m)
		{
			string properties = "";
			foreach (Field f in m.Fields) {
				if (f.OptionDeprecated)
					properties += "[Obsolete]\n";
				properties += f.PropertyType + " " + f.Name + " { get; set; }\n";
			}
			
			string code = "";
			code += "public interface I" + m.CSName + "\n";
			code += "{\n";
			code += Code.Indent (properties);
			code += "}\n";
			return code;
		}

	}
}

