using System;
using System.IO;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    static class ProtoPrepare
    {
        /// <summary>
        /// Convert message/class and field/propery names to CamelCase
        /// </summary>
        public static bool ConvertToCamelCase = true;

        /// <summary>
        /// If the name clashes between a property and subclass, the property will be renamed.
        /// If false, an error will occur.
        /// </summary>
        public static bool FixNameclash = false;
        public const string FixNameclashArgument = "--fix-nameclash";

        static public void Prepare(ProtoCollection file)
        {
            foreach (ProtoMessage m in file.Messages.Values)
            {
                if (m.OptionNamespace == null)
                    m.OptionNamespace = GetCamelCase(m.Package);
                PrepareMessage(m);
            }

            foreach (ProtoEnum e in file.Enums.Values)
            {
                if (e.OptionNamespace == null)
                    e.OptionNamespace = GetCamelCase(e.Package);
                e.CsType = GetCamelCase(e.ProtoName);
            }
        }

        static void PrepareMessage(ProtoMessage m)
        {
            //Name of message and enums
            m.CsType = ProtoPrepare.GetCamelCase(m.ProtoName);
            foreach (ProtoEnum e in m.Enums.Values)
            {
                e.CsType = GetCamelCase(e.ProtoName);
            }
            
            foreach (ProtoMessage sub in m.Messages.Values)
                PrepareMessage(sub);
            
            //Prepare fields
            foreach (Field f in m.Fields.Values)
            {
                PrepareProtoType(m, f);

                DetectNameClash(m, f);

                if (f.OptionDefault != null)
                {
                    if (f.ProtoType is ProtoBuiltin && ((ProtoBuiltin)f.ProtoType).ProtoName == "bytes")
                        throw new NotImplementedException();
                    if (f.ProtoType is ProtoMessage)
                        throw new InvalidDataException("Message can't have a default");
                }
            }   

        }

        /// <summary>
        /// Detect field which have the same name as a submessage in the same message.
        /// </summary>
        /// <param name="m">Parent message</param>
        /// <param name="f">Field to check</param>
        static void DetectNameClash(ProtoMessage m, Field f)
        {  
            bool nameclash = false;
            foreach (var tm in m.Messages.Values)
                if (tm.CsType == f.CsName)
                    nameclash = true;
            foreach (var te in m.Enums.Values)
                if (te.CsType == f.CsName)
                    nameclash = true;
            foreach (var tf in m.Fields.Values)
            {
                if (tf == f)
                    continue;
                if (tf.CsName == f.CsName)
                    nameclash = true;
            }
            if (nameclash == false)
                return;

            //Name clash
            if (FixNameclash)
            {
                if (ConvertToCamelCase)
                    f.CsName += "Field";
                else
                    f.CsName += "_field";


                Console.Error.WriteLine("Warning: renamed field: " + m.FullCsType + "." + f.CsName);

                //Make sure our change did not result in another name collission
                DetectNameClash(m, f);
            } else
                throw new ProtoFormatException("The field: " + m.FullCsType + "." + f.CsName + 
                    " has the same name as a sibling class/enum type which is not allowed in C#. " +
                    "Use " + FixNameclashArgument + " to automatically rename the field.", f.Source);
        }
        
        /// <summary>
        /// Prepare: ProtoType, WireType and CSType
        /// </summary>
        static void PrepareProtoType(ProtoMessage m, Field f)
        {
            //Change property name to C# style, CamelCase.
            f.CsName = GetCSPropertyName(m, f.ProtoName);
            
            f.ProtoType = GetBuiltinProtoType(f.ProtoTypeName);
            if (f.ProtoType == null)
                f.ProtoType = Search.GetProtoType(m, f.ProtoTypeName);
            if (f.ProtoType == null)
            {
#if DEBUG
                //this will still return null but we keep it here for debugging purposes
                f.ProtoType = Search.GetProtoType(m, f.ProtoTypeName);
#endif
                throw new ProtoFormatException("Field type \"" + f.ProtoTypeName + "\" not found for field " + f.ProtoName + " in message " + m.FullProtoName, f.Source);
            }

            if (f.OptionPacked)
            {
                if (f.ProtoType.WireType == Wire.LengthDelimited)
                    throw new InvalidOperationException("Length delimited types cannot be packed");
            }
        }

        /// <summary>
        /// Return the type given the name from a .proto file.
        /// Return Unknonw if it is a message or an enum.
        /// </summary>
        static ProtoBuiltin GetBuiltinProtoType(string type)
        {     
            switch (type)
            {
                case "double":
                    return new ProtoBuiltin(type, Wire.Fixed64, "double");
                case "float":
                    return new ProtoBuiltin(type, Wire.Fixed32, "float");
                case "int32":
                    return new ProtoBuiltin(type, Wire.Varint, "int");
                case "int64":
                    return new ProtoBuiltin(type, Wire.Varint, "long");
                case "uint32":
                    return new ProtoBuiltin(type, Wire.Varint, "uint");
                case "uint64":
                    return new ProtoBuiltin(type, Wire.Varint, "ulong");
                case "sint32":
                    return new ProtoBuiltin(type, Wire.Varint, "int");
                case "sint64":
                    return new ProtoBuiltin(type, Wire.Varint, "long");
                case "fixed32":
                    return new ProtoBuiltin(type, Wire.Fixed32, "uint");
                case "fixed64":
                    return new ProtoBuiltin(type, Wire.Fixed64, "ulong");
                case "sfixed32":
                    return new ProtoBuiltin(type, Wire.Fixed32, "int");
                case "sfixed64":
                    return new ProtoBuiltin(type, Wire.Fixed64, "long");
                case "bool":
                    return new ProtoBuiltin(type, Wire.Varint, "bool");
                case "string":
                    return new ProtoBuiltin(type, Wire.LengthDelimited, "string");
                case "bytes":
                    return new ProtoBuiltin(type, Wire.LengthDelimited, "byte[]");
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the C# CamelCase version of a given name.
        /// Name collisions with enums are avoided.
        /// </summary>
        static string GetCSPropertyName(ProtoMessage m, string name)
        {
            string csname = GetCamelCase(name); 
            
            foreach (ProtoEnum me in m.Enums.Values)
                if (me.CsType == csname)
                    return name;
            
            return csname;          
        }
        
        /// <summary>
        /// Gets the CamelCase version of a given name.
        /// </summary>
        static string GetCamelCase(string name)
        {
            if (ConvertToCamelCase == false)
                return name;

            string csname = "";
            
            if (name.Contains("_") == false)
                csname += name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
            else
            {
                foreach (string part in name.Split('_'))
                {
                    if (part.Length == 0)
                        csname += "_";
                    else
                        csname += part.Substring(0, 1).ToUpperInvariant() + part.Substring(1);
                }
            }       
            
            return csname;          
        }

    }
}

