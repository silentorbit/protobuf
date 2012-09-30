using System;
using System.Collections.Generic;

namespace ProtocolBuffers
{
    /// <summary>
    /// Parses local feature setting from the comments of the .proto file.
    /// </summary>
    internal static class LocalParser
    {
        static void ParseMessageFlags(ProtoMessage message, string flag)
        {
            switch (flag)
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
                    throw new NotImplementedException("Unknown option: " + flag);
            }
        }
        
        static void ParseMessageOption(ProtoMessage message, string key, string value)
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
                    throw new  NotImplementedException("Unknown option: " + key);
            }
        }
        
        static void ParseFieldFlags(Field field, string flag)
        {
            switch (flag)
            {
                case "external":
                    field.OptionExternal = true;
                    break;
                case "readonly":
                    field.OptionReadOnly = true;
                    break;
                default:
                    throw new  NotImplementedException("Unknown field option: " + flag);
            }
        }
        
        static void ParseFieldOption(Field field, string key, string value)
        {
            switch (key)
            {
                case "access":
                    field.OptionAccess = value;
                    break;
                case "codetype":
                    field.OptionCodeType = value;
                    break;
                default:
                    throw new NotImplementedException("Unknown field option: " + key);
            }
        }
        
        public static void ParseComments(IComment message, List<string> comments, TokenReader tr)
        {
            message.Comments = "";
            foreach (string s in comments)
            {
                if (s.StartsWith(":"))
                {
                    try
                    {
                        string line = s.Substring(1);

                        //Remove comments after "//"
                        int cpos = line.IndexOf("//");
                        if (cpos >= 0)
                            line = line.Substring(0, cpos);

                        string[] parts = line.Split('=');
                        if (parts.Length > 2)
                            throw new ProtoFormatException("Bad option format, at most one '=', " + s, tr);
                        string key = parts [0].Trim().ToLowerInvariant();
                        if (parts.Length == 1)
                        {
                            //Parse flag
                            if (message is ProtoMessage)
                                ParseMessageFlags((ProtoMessage)message, key);
                            else if (message is Field)
                                ParseFieldFlags((Field)message, key);
                            else
                                throw new NotImplementedException();
                        
                            continue;
                        } else
                        {
                            string value = (parts.Length == 2) ? parts [1].Trim() : null;

                            if (message is ProtoMessage)
                                ParseMessageOption((ProtoMessage)message, key, value);
                            else if (message is Field)
                                ParseFieldOption((Field)message, key, value);
                            else
                                throw new NotImplementedException();

                            continue;
                        }
                    } catch (Exception e)
                    {
                        throw new ProtoFormatException(e.Message, e, tr);
                    }
                } else
                {
                    message.Comments += s + "\n";
                }
            }
            message.Comments = message.Comments.Trim(new char[]{'\n'}).Replace("\n", "\r\n");
            comments.Clear();
        }

    }
}

