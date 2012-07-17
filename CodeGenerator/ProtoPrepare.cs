using System;
using System.IO;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    static class ProtoPrepare
    {
        static public void Prepare(ProtoCollection file)
        {
            foreach (ProtoMessage m in file.Messages.Values)
            {
                if(m.OptionNamespace == null)
                    m.OptionNamespace = GetCamelCase(m.Package);
                PrepareMessage(m);
            }

            foreach (ProtoEnum e in file.Enums.Values)
            {
                if(e.OptionNamespace == null)
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
        /// Prepare: ProtoType, WireType and CSType
        /// </summary>
        static void PrepareProtoType(ProtoMessage m, Field f)
        {
            //Change property name to C# style, CamelCase.
            f.CsName = GetCSPropertyName(m, f.ProtoName);
            
            f.ProtoType = GetBuiltinProtoType(f.ProtoTypeName);
            if (f.ProtoType == null)
                f.ProtoType = m.GetProtoType(f.ProtoTypeName);
            if (f.ProtoType == null)
            {
#if DEBUG
                //this will still return null but we keep it here for debugging purposes
                f.ProtoType = m.GetProtoType(f.ProtoTypeName);
#endif
                throw new ProtoFormatException("Field type \""+f.ProtoTypeName+"\" not found for field " + f.ProtoName + " in message " + m.FullProtoName);
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
            string csname = "";
            
            if (name.Contains("_") == false)
                csname += name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
            else
            {
                foreach (string part in name.Split('_'))
                {
                    csname += part.Substring(0, 1).ToUpperInvariant() + part.Substring(1);
                }
            }       
            
            return csname;          
        }

    }
}

