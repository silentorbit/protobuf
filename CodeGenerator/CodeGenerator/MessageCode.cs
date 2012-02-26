using System;

namespace ProtocolBuffers
{
	public class MessageCode
	{
		public virtual string GenerateClass (Message m)
		{
			string code = "";

			//Default class
			if (m.Comments != null) {
				code += "/// <summary>\n";
				code += Code.Prefix ("/// ", m.Comments) + "\n";
				code += "/// </summary>\n";
			}
			code += m.OptionAccess + " partial class " + m.CSType + "\n";
			code += "{\n";
			
			string enums = GenerateEnums (m);
			if (enums.Length > 0) {
				code += Code.Indent (enums);
				code += "\n";
			}

			code += Code.Indent (GenerateProperties (m));
			
			if (m.OptionTriggers) {
				code += Code.Indent (Code.Comment (
					"protected virtual void BeforeSerialize () {}\n" +
					"protected virtual void AfterDeserialize () {}\n"));
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
			string code = "";
			foreach (MessageEnum me in m.Enums) {
				code += "public enum " + me.CSType + "\n";
				code += "{\n";
				foreach (var epair in me.Enums) {
					if (me.EnumsComments.ContainsKey (epair.Key)) {
						code += "	/// <summary>\n";
						code += Code.Prefix ("	/// ", me.EnumsComments [epair.Key]) + "\n";
						code += "	/// </summary>\n";
					}
					code += "	" + epair.Key + " = " + epair.Value + ",\n";
				}
				code += "}\n";
			}
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
			foreach (Field f in m.Fields.Values) {
				if (f.OptionGenerate) {
					if (f.Comments != null) {
						code += "/// <summary>\n";
						code += Code.Prefix ("/// ", f.Comments) + "\n";
						code += "/// </summary>\n";
					}
					code += GenerateProperty (f) + "\n\n";
				} else {
					code += "//" + GenerateProperty (f) + "	//Implemented by user elsewhere\n\n";
				}
			}
			return code;
		}
		
		protected virtual string GenerateProperty (Field f)
		{
			if (f.OptionReadOnly)
				return f.OptionAccess + " readonly " + f.PropertyType + " " + f.Name + " = new " + f.PropertyType + "();";
			else
				return f.OptionAccess + " " + f.PropertyType + " " + f.Name + " { get; set; }";
		}
	}
}

