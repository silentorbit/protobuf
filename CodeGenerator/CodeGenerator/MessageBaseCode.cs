using System;

namespace ProtocolBuffers
{
	public class MessageBaseCode : MessageCode
	{
		public override string GenerateClass (Message m)
		{
			string code = "";
			
			//Base class
			code += "public abstract class " + m.CSName + "Base\n";
			code += "{\n";
			code += Code.Indent (GenerateProperties (m));
			code += "\n";
			if (m.OptionTriggers) {
				code += "	protected virtual void BeforeSerialize()\n";
				code += "	{\n";
				code += "	}\n";
				code += "\n";
				code += "	protected virtual void AfterDeserialize()\n";
				code += "	{\n";
				code += "	}\n";
			}
			code += "}\n\n";
			
			//Default class
			code += "public partial class " + m.CSName + " : " + m.CSName + "Base\n";
			code += "{\n";
			string enums = GenerateEnums (m);
			if (enums.Length > 0) {
				code += Code.Indent (enums);
				code += "\n";
			}
			#if !GENERATE_BASE
			code += Code.Indent (GenerateProperties (m));
			code += "\n";
			#endif
			
			foreach (Message sub in m.Messages) {
				code += "\n";
				code += Code.Indent (GenerateClass (sub));
			}
			code += "}\n";
			
			return code;
		}
		
		protected override string GenerateProperty (Field f)
		{
			return f.OptionAccess + " virtual " + f.PropertyType + " " + f.Name + " { get; set; }";
		}
	}
}

