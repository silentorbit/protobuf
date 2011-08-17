using System;

namespace ProtocolBuffers
{
	public class MessageCode
	{
		public virtual string GenerateClass (Message m)
		{
			string code = "";

			//Default class
			code += "public partial class " + m.CSName + "\n";
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

		protected string GenerateEnums (Message m)
		{
			string enums = "";
			foreach (MessageEnum me in m.Enums) {
				enums += "public enum " + me.CSName + "\n";
				enums += "{\n";
				foreach (var epair in me.Enums)
					enums += "	" + epair.Key + " = " + epair.Value + ",\n";
				enums += "}\n";
			}
			return enums;
		}

		public virtual string GenerateClassTemplate (Message m)
		{
			string code = "";

			//Default class
			code += "public partial class " + m.CSName + "\n";
			code += "{\n";

			code += Code.Indent (GenerateTemplateProperties (m));
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
			
			foreach (Message sub in m.Messages) {
				code += "\n";
				code += Code.Indent (GenerateClassTemplate (sub));
			}
			code += "}\n";
			return code;
		}
		
		/// <summary>
		/// Generates the properties.
		/// </summary>
		/// <param name='template'>
		/// if true it will generate only properties that are not included by default, because of the [generate=false] option.
		/// </param>
		protected string GenerateProperties (Message m)
		{
			string code = "";
			foreach (Field f in m.Fields) {
				if (f.OptionGenerate == true)
					code += GenerateProperty (f) + "\n";
			}
			return code;
		}
		
		protected string GenerateTemplateProperties (Message m)
		{
			string code = "";
			foreach (Field f in m.Fields) {
				if (f.OptionGenerate == false)
					code += GenerateProperty (f) + "\n";
			}
			return code;
		}
		
		protected virtual string GenerateProperty (Field f)
		{
			return f.OptionAccess + " " + f.PropertyType + " " + f.Name + " { get; set; }";
		}
	}
}

