using System;

namespace ProtocolBuffers
{
	public static class FieldCode
	{
		#region Reader
		
		public static string GenerateFieldReader (Field f)
		{
			string code = "";
			if (f.Rule == FieldRule.Repeated) {
				if (f.OptionPacked == true) {
					code += "using (MemoryStream ms" + f.ID + " = new MemoryStream(ProtocolParser.ReadBytes(stream))) {\n";
					code += "	while(true)\n";
					code += "	{\n";
					code += "		if (ms" + f.ID + ".Position == ms" + f.ID + ".Length)\n";
					code += "			break;\n";
					code += "		instance." + f.Name + ".Add (" + GenerateFieldTypeReader (f, "ms" + f.ID, "br", null) + ");\n";
					code += "	}\n";
					code += "}\n";
				} else {
					code += "instance." + f.Name + ".Add (" + GenerateFieldTypeReader (f, "stream", "br", null) + ");\n";
				}
			} else {			
				if (f.ProtoType == ProtoTypes.Message) {
					if (f.OptionReadOnly)
						code += GenerateFieldTypeReader (f, "stream", "br", "instance." + f.Name) + ";";
					else {
						code += "if (instance." + f.Name + " == null)\n";
						code += "	instance." + f.Name + " = " + GenerateFieldTypeReader (f, "stream", "br", null) + ";\n";
						code += "else\n";
						code += "	" + GenerateFieldTypeReader (f, "stream", "br", "instance." + f.Name) + ";";
					}
				} else
					code += "instance." + f.Name + " = " + GenerateFieldTypeReader (f, "stream", "br", "instance." + f.Name) + ";";
			}
			return code;
		}

		static string GenerateFieldTypeReader (Field f, string stream, string binaryReader, string instance)
		{
			if (f.OptionCodeType != null) {
				switch (f.OptionCodeType) {
				case "DateTime":
					return "new DateTime((long)ProtocolParser.ReadUInt64 (" + stream + "))";
				case "TimeSpan":
					return "new TimeSpan((long)ProtocolParser.ReadUInt64 (" + stream + "))";
				default:
					//Assume enum
					return "(" + f.OptionCodeType + ")" + GenerateFieldTypeReaderPrimitive (f, stream, instance);
				}
			}
			
			return GenerateFieldTypeReaderPrimitive (f, stream, instance);
		}

		static string GenerateFieldTypeReaderPrimitive (Field f, string stream, string instance)
		{
			switch (f.ProtoType) {
			case ProtoTypes.Double:
				return "br.ReadDouble ()";
			case ProtoTypes.Float:
				return "br.ReadSingle ()";
			case ProtoTypes.Fixed32:
				return "br.ReadUInt32 ()";
			case ProtoTypes.Fixed64:
				return "br.ReadUInt64 ()";
			case ProtoTypes.Sfixed32:
				return "br.ReadInt32 ()";
			case ProtoTypes.Sfixed64:
				return "br.ReadInt64 ()";
			case ProtoTypes.Int32:
				return "(int)ProtocolParser.ReadUInt32 (" + stream + ")";
			case ProtoTypes.Int64:
				return "(long)ProtocolParser.ReadUInt64 (" + stream + ")";
			case ProtoTypes.Uint32:
				return "ProtocolParser.ReadUInt32 (" + stream + ")";
			case ProtoTypes.Uint64:
				return "ProtocolParser.ReadUInt64 (" + stream + ")";
			case ProtoTypes.Sint32:
				return "ProtocolParser.ReadSInt32 (" + stream + ")";
			case ProtoTypes.Sint64:
				return "ProtocolParser.ReadSInt64 (" + stream + ")";
			case ProtoTypes.Bool:
				return "ProtocolParser.ReadBool (" + stream + ")";
			case ProtoTypes.String:
				return "ProtocolParser.ReadString (" + stream + ")";
			case ProtoTypes.Bytes:
				return "ProtocolParser.ReadBytes (" + stream + ")";
			case ProtoTypes.Enum:
				return "(" + f.PropertyItemType + ")ProtocolParser.ReadUInt32 (" + stream + ")";
			case ProtoTypes.Message:				
				if (f.Rule == FieldRule.Repeated)
					return f.FullPath + ".Deserialize (ProtocolParser.ReadBytes (" + stream + "))";
				else {
					if (instance == null)
						return f.FullPath + ".Deserialize (ProtocolParser.ReadBytes (" + stream + "))";
					else
						return f.FullPath + ".Deserialize (ProtocolParser.ReadBytes (" + stream + "), " + instance + ")";
				}
			default:
				throw new NotImplementedException ();
			}
		}

		#endregion
		
		
		
		#region Writer
		
		/// <summary>
		/// Generates code for writing one field
		/// </summary>
		public static string GenerateFieldWriter (Message m, Field f)
		{
			string code = "";
			if (f.Rule == FieldRule.Repeated) {
				if (f.OptionPacked == true) {
					
					string binaryWriter = "";
					switch (f.ProtoType) {
					case ProtoTypes.Double:
					case ProtoTypes.Float:
					case ProtoTypes.Fixed32:
					case ProtoTypes.Fixed64:
					case ProtoTypes.Sfixed32:
					case ProtoTypes.Sfixed64:
						binaryWriter = "\nBinaryWriter bw" + f.ID + " = new BinaryWriter(ms" + f.ID + ");";
						break;
					}
					
					code += "if (instance." + f.Name + " != null) {\n";
					code += "	ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
					code += "	using (MemoryStream ms" + f.ID + " = new MemoryStream())\n";
					code += "	{	" + binaryWriter + "\n";
					code += "		foreach (" + f.PropertyItemType + " i" + f.ID + " in instance." + f.Name + ") {\n";
					code += Code.Indent (3, GenerateFieldTypeWriter (f, "ms" + f.ID, "bw" + f.ID, "i" + f.ID)) + "\n";
					code += "		}\n";
					code += "		ProtocolParser.WriteBytes (stream, ms" + f.ID + ".ToArray ());\n";
					code += "	}\n";
					code += "}\n";
					return code;
				} else {
					code += "if (instance." + f.Name + " != null) {\n";
					code += "	foreach (" + f.PropertyItemType + " i" + f.ID + " in instance." + f.Name + ") {\n";
					code += "		ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
					code += Code.Indent (2, GenerateFieldTypeWriter (f, "stream", "bw", "i" + f.ID)) + "\n";
					code += "	}\n";
					code += "}\n";
					return code;
				}
			} else if (f.Rule == FieldRule.Optional) {			
				switch (f.ProtoType) {
				case ProtoTypes.String:
				case ProtoTypes.Message:
				case ProtoTypes.Bytes:
					code += "if (instance." + f.Name + " != null) {\n";
					code += "	ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
					code += Code.Indent (GenerateFieldTypeWriter (f, "stream", "bw", "instance." + f.Name));
					code += "}\n";
					return code;
				case ProtoTypes.Enum:
					code += "if (instance." + f.Name + " != " + f.PropertyItemType + "." + f.OptionDefault + ") {\n";
					code += "	ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
					code += Code.Indent (GenerateFieldTypeWriter (f, "stream", "bw", "instance." + f.Name));
					code += "}\n";
					return code;
				default:
					code += "ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
					code += GenerateFieldTypeWriter (f, "stream", "bw", "instance." + f.Name);
					return code;
				}
			} else if (f.Rule == FieldRule.Required) {			
				switch (f.ProtoType) {
				case ProtoTypes.String:
				case ProtoTypes.Message:
				case ProtoTypes.Bytes:
					code += "if (instance." + f.Name + " == null)\n";
					code += "	throw new ArgumentNullException (\"" + f.Name + "\", \"Required by proto specification.\");\n";
					break;
				}
				code += "ProtocolParser.WriteKey (stream, new ProtocolBuffers.Key (" + f.ID + ", Wire." + f.WireType + "));\n";
				code += GenerateFieldTypeWriter (f, "stream", "bw", "instance." + f.Name);
				return code;
			}			
			throw new NotImplementedException ("Unknown rule: " + f.Rule);
		}
					
		public static string GenerateFieldTypeWriter (Field f, string stream, string binaryWriter, string instance)
		{
			if (f.OptionCodeType != null) {
				switch (f.OptionCodeType) {
				case "DateTime":
				case "TimeSpan":
					return "ProtocolParser.WriteUInt64 (" + stream + ", (ulong)" + instance + ".Ticks);\n";
				default: //enum
					break;
				}
			}
			
			switch (f.ProtoType) {
			case ProtoTypes.Double:
			case ProtoTypes.Float:
			case ProtoTypes.Fixed32:
			case ProtoTypes.Fixed64:
			case ProtoTypes.Sfixed32:
			case ProtoTypes.Sfixed64:
				return binaryWriter + ".Write (" + instance + ");\n";
			case ProtoTypes.Int32:
				return "ProtocolParser.WriteUInt32 (" + stream + ", (uint)" + instance + ");\n";
			case ProtoTypes.Int64:
				return "ProtocolParser.WriteUInt64 (" + stream + ", (ulong)" + instance + ");\n";
			case ProtoTypes.Uint32:
				return "ProtocolParser.WriteUInt32 (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Uint64:
				return "ProtocolParser.WriteUInt64 (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Sint32:
				return "ProtocolParser.WriteSInt32 (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Sint64:
				return "ProtocolParser.WriteSInt64 (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Bool:
				return "ProtocolParser.WriteBool (" + stream + ", " + instance + ");\n";
			case ProtoTypes.String:
				return "ProtocolParser.WriteString (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Bytes:
				return "ProtocolParser.WriteBytes (" + stream + ", " + instance + ");\n";
			case ProtoTypes.Enum:
				return "ProtocolParser.WriteUInt32 (" + stream + ", (uint)" + instance + ");\n";
			case ProtoTypes.Message:				
				string code = "";
				code += "using (MemoryStream ms" + f.ID + " = new MemoryStream()) {\n";
				code += "	" + f.FullPath + ".Serialize (ms" + f.ID + ", " + instance + ");\n";
				code += "	ProtocolParser.WriteBytes (" + stream + ", ms" + f.ID + ".ToArray ());\n";
				code += "}\n";
				return code;
			default:
				throw new NotImplementedException ();
			}
		}
		
		#endregion
	}
}

