using System;

namespace ProtocolBuffers
{
	public static class SerializerCode
	{
		public static string GenerateClassSerializer (Message m)
		{
			string code = "";
			code += "public partial class " + m.CSName + "\n";
			code += "{\n";
			code += Code.Indent (GenerateReader (m));
			code += "\n";
			code += Code.Indent (GenerateWriter (m));
			foreach (Message sub in m.Messages) {
				code += "\n";
				code += Code.Indent (GenerateClassSerializer (sub));
			}
			code += "}\n";
			code += "\n";
			return code;
		}
		
		public static string GenerateGenericClassSerializer (Message m)
		{
			string code = "";
			code += "\n";
			code += GenerateGenericReader (m);
			code += "\n";
			code += GenerateGenericWriter (m);
			code += "\n";
			foreach (Message sub in m.Messages) {
				code += "\n";
				code += GenerateGenericClassSerializer (sub);
			}
			return code;
		}
		
		static string GenerateReader (Message m)
		{
			string code = "";
			code += "public static " + m.CSName + " Deserialize(Stream stream)\n";
			code += "{\n";
			code += "	" + m.CSName + " instance = new " + m.CSName + "();\n";
			code += "	Deserialize(stream, instance);\n";
			code += "	return instance;\n";
			code += "}\n";
			code += "\n";
			code += "public static " + m.CSName + " Deserialize(byte[] buffer)\n";
			code += "{\n";
			code += "	using(MemoryStream ms = new MemoryStream(buffer))\n";
			code += "		return Deserialize(ms);\n";
			code += "}\n";
			code += "\n";
			code += "public static T Deserialize<T> (Stream stream) where T : " + m.FullPath + ", new()\n";
			code += "{\n";
			code += "	T instance = new T ();\n";
			code += "	Deserialize (stream, instance);\n";
			code += "	return instance;\n";
			code += "}\n";
			code += "\n";
			code += "public static T Deserialize<T> (byte[] buffer) where T : " + m.FullPath + ", new()\n";
			code += "{\n";
			code += "	T instance = new T ();\n";
			code += "	Deserialize(buffer, instance);\n";
			code += "	return instance;\n";
			code += "}\n";
			code += "\n";
			code += "public static void Deserialize (byte[] buffer, " + m.FullPath + " instance)\n";
			code += "{\n";
			code += "	using (MemoryStream ms = new MemoryStream(buffer))\n";
			code += "		Deserialize (ms, instance);\n";
			code += "}\n";
			code += "\n";
			
			code += "public static " + m.FullPath + " Deserialize(Stream stream, " + m.FullPath + " instance)\n";
			code += "{\n";
			foreach (Field f in m.Fields) {
				if (f.WireType == Wire.Fixed32 || f.WireType == Wire.Fixed64) {
					code += "	BinaryReader br = new BinaryReader (stream);";
					break;
				}
			}
			
			foreach (Field f in m.Fields) {
				if (f.Rule == Rules.Repeated) {
					code += "	if(instance." + f.Name + " == null)\n";
					code += "		instance." + f.Name + " = new List<" + f.PropertyItemType + ">();\n";
				} else if (f.OptionDefault != null) {
					if (f.ProtoType == ProtoTypes.Enum)
						code += "	instance." + f.Name + " = " + f.FullPath + "." + f.OptionDefault + ";\n";
					else
						code += "	instance." + f.Name + " = " + f.OptionDefault + ";\n";
				} else if (f.Rule == Rules.Optional) {
					if (f.ProtoType == ProtoTypes.Enum) {
						//the default value is the first value listed in the enum's type definition
						foreach (var kvp in f.ProtoTypeEnum.Enums) {
							code += "	instance." + f.Name + " = " + kvp.Key + ";\n";
							break;
						}
					}
					if (f.ProtoType == ProtoTypes.String) {
						code += "	instance." + f.Name + " = \"\";\n";
					}
				}
			}

			code += "	while (true)\n";
			code += "	{\n";
			code += "		ProtocolBuffers.Key key = null;\n";
			code += "		try {\n";
			code += "			key = ProtocolParser.ReadKey (stream);\n";
			code += "		} catch (IOException) {\n";
			code += "			break;\n";
			code += "		}\n";
			code += "\n";
			code += "		switch (key.Field) {\n";
			code += "		case 0:\n";
			code += "			throw new InvalidDataException(\"Invalid field id: 0, something went wrong in the stream\");\n";
			foreach (Field f in m.Fields) {
				code += "		case " + f.ID + ":\n";
				code += Code.Indent (3, FieldCode.GenerateFieldReader (f)) + "\n";
				code += "			break;\n";
			}
			code += "		default:\n";
			code += "			ProtocolParser.SkipKey(stream, key);\n";
			code += "			break;\n";
			code += "		}\n";
			code += "	}\n";
			code += "	\n";
			if (m.OptionTriggers)
				code += "	instance.AfterDeserialize();\n";
			code += "	return instance;\n";
			code += "}\n";
			code += "\n";
			code += "public static " + m.FullPath + " Read(byte[] buffer, " + m.FullPath + " instance)\n";
			code += "{\n";
			code += "	using (MemoryStream ms = new MemoryStream(buffer))\n";
			code += "		Deserialize (ms, instance);\n";
			code += "	return instance;\n";
			code += "}\n";

			return code;
		}

		static string GenerateGenericReader (Message m)
		{
			string code = "";
			code += "public static " + m.FullPath + " Read (Stream stream, " + m.FullPath + " instance)\n";
			code += "{\n";
			code += "	return " + m.FullPath + ".Deserialize(stream, instance);\n";
			code += "}\n";
			code += "\n";
			code += "public static " + m.FullPath + " Read(byte[] buffer, " + m.FullPath + " instance)\n";
			code += "{\n";
			code += "	using (MemoryStream ms = new MemoryStream(buffer))\n";
			code += "		" + m.FullPath + ".Deserialize (ms, instance);\n";
			code += "	return instance;\n";
			code += "}\n";
			return code;
		}
		
		
		/// <summary>
		/// Generates code for writing a class/message
		/// </summary>
		static string GenerateWriter (Message m)
		{
			string code = "public static void Serialize(Stream stream, " + m.CSName + " instance)\n";
			code += "{\n";
			if (m.OptionTriggers) {
				code += "	instance.BeforeSerialize();\n";
				code += "\n";
			}
			if (GenerateBinaryWriter (m))
				code += "	BinaryWriter bw = new BinaryWriter(stream);\n";
			
			foreach (Field f in m.Fields) {
				code += Code.Indent (FieldCode.GenerateFieldWriter (m, f));
			}
			code += "}\n\n";
			
			code += "public static byte[] SerializeToBytes(" + m.CSName + " instance)\n";
			code += "{\n";
			code += "	using(MemoryStream ms = new MemoryStream())\n";
			code += "	{\n";
			code += "		Serialize(ms, instance);\n";
			code += "		return ms.ToArray();\n";
			code += "	}\n";
			code += "}\n";
			
			return code;
		}
		
		/// <summary>
		/// Generates code for writing a class/message
		/// </summary>
		static string GenerateGenericWriter (Message m)
		{
			string code = "";
			code += "public static void Write(Stream stream, " + m.FullPath + " instance)\n";
			code += "{\n";
			code += "	" + m.FullPath + ".Serialize(stream, instance);\n";
			code += "}\n";
			return code;
		}
		
		/// <summary>
		/// Adds BinaryWriter only if it will be used
		/// </summary>
		static bool GenerateBinaryWriter (Message m)
		{
			foreach (Field f in m.Fields) {
				if (f.WireType == Wire.Fixed32 || f.WireType == Wire.Fixed64) {
					return true;
				}
			}
			return false;
		}
	}
}

