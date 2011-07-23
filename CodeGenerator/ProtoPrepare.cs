using System;
using System.IO;

namespace ProtocolBuffers
{
	public static class ProtoPrepare
	{
		/// <summary>
		/// Prepare: ProtoType, WireType and CSType
		/// </summary>
		public static void PrepareProtoType (Message m, Field f)
		{
			//Change property name to C# style, CamelCase.
			f.Name = GetCSPropertyName (m, f.Name);
			
			string name = null;
			
			f.ProtoType = GetScalarProtoType (f.ProtoTypeName);
						
			//Wire, and set type
			switch (f.ProtoType) {
			case ProtoTypes.Double:
			case ProtoTypes.Fixed64:
			case ProtoTypes.Sfixed64:
				f.WireType = Wire.Fixed64;
				break;
			case ProtoTypes.Float:
			case ProtoTypes.Fixed32:
			case ProtoTypes.Sfixed32:
				f.WireType = Wire.Fixed32;
				break;
			case ProtoTypes.Int32:
			case ProtoTypes.Int64:
			case ProtoTypes.Uint32:
			case ProtoTypes.Uint64:
			case ProtoTypes.Sint32:
			case ProtoTypes.Sint64:
			case ProtoTypes.Bool:
				f.WireType = Wire.Varint;
				break;
			case ProtoTypes.String:
			case ProtoTypes.Bytes:
				f.WireType = Wire.LengthDelimited;
				break;
			default:
				if (f.ProtoTypeName.Contains (".")) {
					f.ProtoType = ProtoTypes.Enum;
					string[] parts = f.ProtoTypeName.Split ('.');
					name = GetCamelCase (parts [0]) + "." + GetCamelCase (parts [1]);
					f.WireType = Wire.Varint; //enum
					break;
				}
				if (m.Enums.ContainsKey (f.ProtoTypeName)) {
					f.ProtoType = ProtoTypes.Enum;
					name = m.Name + "." + GetCamelCase (f.ProtoTypeName);
					f.WireType = Wire.Varint; //enum
					break;
				}
			
				f.ProtoType = ProtoTypes.Message;
				name = f.ProtoTypeName;
				f.WireType = Wire.LengthDelimited; //message
				break;
			}
			
			if (f.Packed) {
				if (f.WireType == Wire.LengthDelimited)
					throw new InvalidDataException ("Packed field not allowed for length delimited types");
				f.WireType = Wire.LengthDelimited;
			}
			
			f.CSType = GetCSType (f.ProtoType, name);
			f.CSItemType = f.CSType;
			if (f.Rule == Rules.Repeated) {
				if (f.ProtoType == ProtoTypes.Message)
					f.CSType = "List<I" + f.CSType + ">";
				else
					f.CSType = "List<" + f.CSType + ">";
			}
			
		}
		
		/// <summary>
		/// Return the type given the name from a .proto file.
		/// Return Unknonw if it is a message or an enum.
		/// </summary>
		static ProtoTypes GetScalarProtoType (string type)
		{
			switch (type) {
			case "double":
				return ProtoTypes.Double;
			case "float":
				return ProtoTypes.Float;
			case "int32":
				return ProtoTypes.Int32;
			case "int64":
				return ProtoTypes.Int64;
			case "uint32":
				return ProtoTypes.Uint32;
			case "uint64":
				return ProtoTypes.Uint64;
			case "sint32":
				return ProtoTypes.Sint32;
			case "sint64":
				return ProtoTypes.Sint64;
			case "fixed32":
				return ProtoTypes.Fixed32;
			case "fixed64":
				return ProtoTypes.Fixed64;
			case "sfixed32":
				return ProtoTypes.Sfixed32;
			case "sfixed64":
				return ProtoTypes.Sfixed64;
			case "bool":
				return ProtoTypes.Bool;
			case "string":
				return ProtoTypes.String;
			case "bytes":
				return ProtoTypes.Bytes;
			default:
				return ProtoTypes.Unknown;
			}
		}
			
		/// <summary>
		/// Gets the c# representation for a given .proto type.
		/// </summary>
		/// <param name='name'>
		/// Name of message or enum, null otherwise
		/// </param>
		private static string GetCSType (ProtoTypes type, string name)
		{
			switch (type) {
			case ProtoTypes.Double:
				return "double";
			case ProtoTypes.Float:
				return "float";
			case ProtoTypes.Int32:
			case ProtoTypes.Sint32:
			case ProtoTypes.Sfixed32:
				return "int";
			case ProtoTypes.Int64:
			case ProtoTypes.Sint64:
			case ProtoTypes.Sfixed64:
				return "long";
			case ProtoTypes.Uint32:
			case ProtoTypes.Fixed32:
				return "uint";
			case ProtoTypes.Uint64:
			case ProtoTypes.Fixed64:
				return "ulong";
			case ProtoTypes.Bool:
				return "bool";
			case ProtoTypes.String:
				return "string";
			case ProtoTypes.Bytes:
				return "byte[]";
				
			case ProtoTypes.Enum:
			case ProtoTypes.Message:
				return GetCamelCase (name);
				
			default:
				throw new NotImplementedException ();
			}
		}
		
		/// <summary>
		/// Get the default value in c# form
		/// </summary>
		public static string GetCSDefaultValue (Field f)
		{
			switch (f.ProtoType) {
			case ProtoTypes.Double:
			case ProtoTypes.Float:
			case ProtoTypes.Fixed32:
			case ProtoTypes.Fixed64:
			case ProtoTypes.Sfixed32:
			case ProtoTypes.Sfixed64:
			case ProtoTypes.Int32:
			case ProtoTypes.Int64:
			case ProtoTypes.Uint32:
			case ProtoTypes.Uint64:
			case ProtoTypes.Sint32:
			case ProtoTypes.Sint64:
			case ProtoTypes.Bool:
				return f.Default;
			
			case ProtoTypes.String:
				return f.Default;
				
			case ProtoTypes.Bytes:
				throw new NotImplementedException ();
				
			case ProtoTypes.Enum:	
				return f.CSType + "." + f.Default;
				
			case ProtoTypes.Message:
				throw new InvalidDataException ("Don't think there can be a default for messages");
				
			default:
				throw new NotImplementedException ();
			}
		}
		
		/// <summary>
		/// Gets the C# CamelCase version of a given name.
		/// Name collisions with enums are avoided.
		/// </summary>
		private static string GetCSPropertyName (Message m, string name)
		{
			string csname = GetCamelCase (name);	
			
			if (m.Enums.ContainsKey (csname))
				return name;
			
			return csname;			
		}
		
		/// <summary>
		/// Gets the CamelCase version of a given name.
		/// </summary>
		public static string GetCamelCase (string name)
		{
			string csname = "";
			
			if (name.Contains ("_") == false)
				csname += name.Substring (0, 1).ToUpperInvariant () + name.Substring (1);
			else {
				foreach (string part in name.Split('_')) {
					csname += part.Substring (0, 1).ToUpperInvariant () + part.Substring (1);
				}
			}		
			
			return csname;			
		}

	}
}

