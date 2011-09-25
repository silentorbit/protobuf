using System;

namespace ProtocolBuffers
{
	public class MessageCode
	{
		public virtual string GenerateClass (Message m)
		{
			string code = "";

			//Default class
			code += m.OptionAccess + " partial class " + m.CSType + "\n";
			code += "{\n";
			
			string enums = GenerateEnums (m);
			if (enums.Length > 0) {
				code += Code.Indent (enums);
				code += "\n";
			}

			code += Code.Indent (GenerateProperties (m));
			code += "\n";
			
			if (m.OptionTriggers) {
				code += Code.Indent (Code.Comment (
					"protected virtual void BeforeSerialize() {}\n" +
					"protected virtual void AfterDeserialize() {}\n"));
				code += "\n";
			}
			
			foreach (Message sub in m.Messages) {
				code += Code.Indent (GenerateClass (sub));
				code += "\n";
			}
			code = code.TrimEnd ('\n');
			code += "\n}\n";
			return code;
		}

		protected string GenerateEnums (Message m)
		{
			string enums = "";
			foreach (MessageEnum me in m.Enums) {
				enums += "public enum " + me.CSType + "\n";
				enums += "{\n";
				foreach (var epair in me.Enums)
					enums += "	" + epair.Key + " = " + epair.Value + ",\n";
				enums += "}\n";
			}
			return enums;
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
			foreach (Field f in m.Fields.Values) {
				if (f.OptionGenerate)
					code += GenerateProperty (f) + "\n";
				else
					code += "//" + GenerateProperty (f) + "	//Implemented by user elsewhere\n";
			}
			return code;
		}
		
		protected virtual string GenerateProperty (Field f)
		{
			return f.OptionAccess + " " + f.PropertyType + " " + f.Name + " { get; set; }";
		}
	}
}

