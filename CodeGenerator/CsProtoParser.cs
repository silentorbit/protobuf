using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    static class CsProtoParser
    {
        public static void Parse(string path, ProtoCollection p)
        {
            Dictionary<string, ProtoType> types = new Dictionary<string, ProtoType>();
            UpdateProtoIndex(p, types);

            using (CsProtoReader reader = new CsProtoReader(path))
            {
                Level level = Level.File;
                string[] prefix = new string[4];
                ProtoMessage message = null;
                Field field = null;

                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;

                    //Get prefix
                    string space = line.Substring(0, line.Length - line.TrimStart(' ', '\t').Length);
                    line = line.Trim();

                    //Match to prefix
                    for (int n = 0; n <= (int)level; n++)
                    {
                        if (prefix [n] == null)
                        {
                            prefix [n] = space;
                            break;
                        }

                        if (space == "")
                            level = (Level)n;

                        //Remove prefix part and continue
                        if (space.StartsWith(prefix [n]))
                            space = space.Substring(prefix [n].Length);
                        else
                            throw new ProtoFormatException("mismatch in indentation", reader);
                    }

                    //Parse line
                    if (level == Level.File)
                    {
                        if (line == "{")
                        {
                            if (message == null)
                                throw new ProtoFormatException("Expected message before {", reader);
                            level = Level.MessageFields;
                            continue;
                        }
                        if (line == "}")
                        {
                            message = null;
                            level = Level.File;
                            continue;
                        }
                        
                        string messageToken = "message";
                        if (line.StartsWith(messageToken) == false)
                            throw new ProtoFormatException("Expected 'message'", reader);

                        //Found message
                        string messageName = line.Substring(messageToken.Length).Trim();
                        message = p.GetProtoType(messageName) as ProtoMessage;
                        if (message == null)
                            throw new ProtoFormatException("Message \"" + messageName + "\" in .csproto not found in .proto", reader);
                        level = Level.Message;
                        continue;
                    }
                    if (level == Level.Message)
                    {
                        //Parse message options
                        string[] parts = line.Split('=');
                        if (parts.Length > 2)
                            throw new ProtoFormatException("Bad option format, at most one '=', " + line, reader);
                        string key = parts [0].Trim().ToLowerInvariant();
                        string value = (parts.Length == 2) ? parts [1].Trim() : null;

                        if (parts.Length == 1)
                        {
                            //Parse flag
                            switch (key)
                            {
                                case "triggers":
                                    message.OptionTriggers = true;
                                    break;
                                case "preserveunknown":
                                    message.OptionPreserveUnknown = true;
                                    break;
                                case "external":
                                    message.OptionExternal = true;
                                    break;
                                default:
                                    throw new ProtoFormatException("Unknown option: " + parts [0], reader);
                            }

                            continue;
                        } else
                        {
                            //Parse value
                            switch (key)
                            {
                                case "namespace":
                                    message.OptionNamespace = value;
                                    break;
                                case "access":
                                    message.OptionAccess = value;
                                    break;
                                case "type":
                                    message.OptionType = value;
                                    break;
                                default:
                                    throw new ProtoFormatException("Unknown option: " + parts [0], reader);
                            }
                            continue;
                        }
                    }
                    if (level == Level.MessageFields)
                    {
                        field = null;
                        foreach (var f in message.Fields)
                        {
                            if (f.Value.ProtoName == line)
                            {
                                field = f.Value;
                                break;
                            }
                        }
                        if (field == null)
                            throw new ProtoFormatException("Field name not found in .proto: " + line, reader);
                        level = Level.Field;
                        continue;
                    }
                    if (level == Level.Field)
                    {
                        //Parse message options
                        string[] parts = line.Split('=');
                        if (parts.Length > 2)
                            throw new ProtoFormatException("Bad option format, at most one '=', " + line, reader);
                        string key = parts [0].Trim().ToLowerInvariant();
                        string value = (parts.Length == 2) ? parts [1].Trim() : null;

                        if (value == null)
                        {
                            //Parse flag
                            switch (key)
                            {
                                case "external":
                                    field.OptionExternal = true;
                                    break;
                                case "readonly":
                                    field.OptionReadOnly = true;
                                    break;
                                default:
                                    throw new ProtoFormatException("Unknown field option: " + parts [0], reader);
                            }
                            continue;
                        } else
                        {
                            //Parse value
                            switch (key)
                            {
                                case "access":
                                    field.OptionAccess = value;
                                    break;
                                case "codetype":
                                    field.OptionCodeType = value;
                                    break;
                                default:
                                    throw new ProtoFormatException("Unknown field option: " + parts [0], reader);
                            }
                            continue;
                        }
                    }
                    
                    throw new NotImplementedException("Level: " + level);
                }
            }
        }

        enum Level
        {
            File = 0,       //Read message
            Message,        //Read message options until {
            MessageFields,  //Read fields
            Field,          //Read field options
        }

        static void UpdateProtoIndex(ProtoMessage pm, Dictionary<string, ProtoType> types)
        {
            foreach (ProtoMessage m in pm.Messages.Values)
            {
                types.Add(m.FullProtoName, m);
                UpdateProtoIndex(m, types);
            }
            foreach (ProtoEnum e in pm.Enums.Values)
            {
                types.Add(e.FullProtoName, e);
            }
        }
        

    }
}

