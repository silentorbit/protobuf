using System;

namespace ProtocolBuffers
{
	/// <summary>
	/// This is currently not used.
	/// This approach place all fields and template methods in a base class.
	/// </summary>
	public class MessageBaseCode : MessageCode
	{
		public override string GenerateClass (Message m)
		{
			string code = "";
			
			//Base class
			code += m.OptionAccess + " abstract class " + m.CSType + "Base\n";
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
			code += m.OptionAccess + " partial class " + m.CSType + " : " + m.CSType + "Base\n";
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

