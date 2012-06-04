using System;
using System.IO;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    static class ProtoPrepare
    {
        public static void Prepare(ProtoFile proto)
        {
            foreach (ProtoMessage m in proto.Messages)
                PrepareMessage(m);
        }
        
        static void PrepareMessage(ProtoMessage m)
        {
            //Name of message and enums
            m.CsType = ProtoPrepare.GetCamelCase(m.ProtoName);
            foreach (ProtoEnum e in m.Enums)
            {
                e.CsType = GetCamelCase(e.ProtoName);
            }
            
            foreach (ProtoMessage sub in m.Messages)
                PrepareMessage(sub);
            
            //Prepare fields
            foreach (Field f in m.Fields.Values)
            {
                PrepareProtoType(m, f);
                if (f.OptionDefault != null)
                {
                    if(f.ProtoType is ProtoBuiltin && ((ProtoBuiltin)f.ProtoType).ProtoName == "bytes")
                        throw new NotImplementedException();
                    if(f.ProtoType is ProtoMessage)
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
            f.Name = GetCSPropertyName(m, f.Name);
            
            f.ProtoType = GetBuiltinProtoType(f.ProtoTypeName);
            if (f.ProtoType == null)
                f.ProtoType = GetProtoType(m, f.ProtoTypeName);
            if (f.ProtoType == null)
                throw new ProtoFormatException("ProtoType not found: " + f.ProtoTypeName);

            if (f.OptionPacked)
            {
                if(f.ProtoType.WireType == Wire.LengthDelimited)
                    throw new InvalidOperationException("Length delimited types cannot be packed");
            }
        }
        
        /// <summary>
        /// Search for field name in message hierarchy
        /// </summary>
        static ProtoType GetProtoType(ProtoMessage m, string path)
        {
            string[] parts = path.Split('.');
            return SearchMessageUp(m, parts);           
        }

        /// <summary>
        /// Searchs the message for matchink classes
        /// </summary>
        /// <param name='name'>
        /// name from .proto
        /// </param>
        static ProtoType SearchMessageUp(ProtoMessage p, string[] name)
        {
            if (p is ProtoFile)
                return SearchMessageDown(p, name);
            
            ProtoMessage m = p as ProtoMessage;
            if (m.ProtoName == name [0])
            {
                if (name.Length == 1)
                    return m;
                
                string[] subName = new string[name.Length - 1];
                Array.Copy(name, 1, subName, 0, subName.Length);
                
                return SearchMessageDown(m, subName);
            }
            
            ProtoType down = SearchMessageDown(p, name);
            if (down != null)
                return down;
            
            return SearchMessageUp(m.Parent, name);
        }
        
        /// <summary>
        /// Search down for matching name
        /// </summary>
        /// <param name='name'>
        /// Split .proto type name
        /// </param>
        static ProtoType SearchMessageDown(ProtoMessage p, string[] name)
        {
            if (name.Length == 1)
            {
                foreach (ProtoEnum me in p.Enums)
                {
                    if (me.ProtoName == name [0])
                        return me;
                }
            }
            
            foreach (ProtoMessage sub in p.Messages)
            {
                if (sub.ProtoName == name [0])
                {
                    if (name.Length == 1)
                        return sub;
                    string[] subName = new string[name.Length - 1];
                    Array.Copy(name, 1, subName, 0, subName.Length);
                    
                    return SearchMessageDown(sub, subName);
                }
            }
            
            return null;
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
            
            foreach (ProtoEnum me in m.Enums)
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

