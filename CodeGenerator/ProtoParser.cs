using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    static class ProtoParser
    {
        /// <summary>
        /// Parse a single .proto file.
        /// Return true if successful/no errors.
        /// </summary>
        public static void Parse(string path, ProtoCollection p)
        {   
            //Preparation for parsing
            //Real parsing is done in ParseMessages
            lastComment.Clear();

            //Read entire file and pass it into a TokenReader
            string t = "";
            using (TextReader reader = new StreamReader(path, Encoding.UTF8))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                        break;
                    
                    t += line + "\n";
                }
            }
            TokenReader tr = new TokenReader(t, path);

            try
            {
                ParseMessages(tr, p);
            } catch (EndOfStreamException)
            {
                return;
            }

        }

        static readonly List<string> lastComment = new List<string>();

        /// <summary>
        /// Return true if token was a comment
        /// </summary>
        static bool ParseComment(string token)
        {
            if (token.StartsWith("//") == false)
                return false;
            token = token.TrimStart('/');
            lastComment.Add(token);
            return true;        
        }
        
        static void ParseMessages(TokenReader tr, ProtoCollection p)
        {
            string package = "Example";

            while (true)
            {
                string token = tr.ReadNext();
                if (ParseComment(token))
                    continue;

                try
                {
                    switch (token)
                    {
                        case ";":
                            lastComment.Clear();
                            continue;
                        case "message":
                            ParseMessage(tr, p, package);
                            break;
                        case "enum":
                            ParseEnum(tr, p, package);
                            break;
                        case "option":
                        //Save options
                            ParseOption(tr, p);
                            break;
                        case "import": //Ignored
                            tr.ReadNext();
                            tr.ReadNextOrThrow(";");
                            break;
                        case "package":
                            package = tr.ReadNext();
                            tr.ReadNextOrThrow(";");
                            break;
                        case "extend":
                            ParseExtend(tr, p, package);
                            break;
                        default:
                            throw new ProtoFormatException("Unexpected/not implemented: " + token, tr);
                    }
                } catch (EndOfStreamException)
                {
                    throw new ProtoFormatException("Unexpected EOF", tr);
                }
            }
        }

        static void ParseMessage(TokenReader tr, ProtoMessage parent, string package)
        {
            var msg = new ProtoMessage(parent, package);
            LocalParser.ParseComments(msg, lastComment, tr);
            msg.ProtoName = tr.ReadNext();

            tr.ReadNextOrThrow("{");

            try
            {
                while (ParseField (tr, msg))
                    continue;
            } catch (Exception e)
            {
                throw new ProtoFormatException(e.Message, e, tr);
            }

            parent.Messages.Add(msg.ProtoName, msg);
        }

        static void ParseExtend(TokenReader tr, ProtoMessage parent, string package)
        {
            var msg = new ProtoMessage(parent, package);
            LocalParser.ParseComments(msg, lastComment, tr);
            msg.ProtoName = tr.ReadNext();

            tr.ReadNextOrThrow("{");

            try
            {
                while (ParseField (tr, msg))
                    continue;
            } catch (Exception e)
            {
                throw new ProtoFormatException(e.Message, e, tr);
            }

            //Not implemented
            //parent.Messages.Add(msg.ProtoName, msg);
        }

        static bool ParseField(TokenReader tr, ProtoMessage m)
        {
            string rule = tr.ReadNext();
            while (true)
            {
                if (ParseComment(rule) == false)
                    break;
                rule = tr.ReadNext();
            }

            Field f = new Field(tr);

            //Rule
            switch (rule)
            {
                case ";":
                    lastComment.Clear();
                    return true;
                case "}":
                    lastComment.Clear();
                    return false;
                case "required":
                    f.Rule = FieldRule.Required;
                    break;
                case "optional":
                    f.Rule = FieldRule.Optional;
                    break;
                case "repeated":
                    f.Rule = FieldRule.Repeated;
                    break;
                case "option":
                //Save options
                    ParseOption(tr, m);
                    return true;
                case "message":
                    ParseMessage(tr, m, m.Package + "." + m.ProtoName);
                    return true;
                case "enum":
                    ParseEnum(tr, m, m.Package + "." + m.ProtoName);
                    return true;
                case "extensions":
                    ParseExtensions(tr, m);
                    return true;
                default:
                    throw new ProtoFormatException("unknown rule: " + rule, tr);
            }

            //Field comments
            LocalParser.ParseComments(f, lastComment, tr);

            //Type
            f.ProtoTypeName = tr.ReadNext();
            
            //Name
            f.ProtoName = tr.ReadNext();
            
            //ID
            tr.ReadNextOrThrow("=");
            f.ID = int.Parse(tr.ReadNext());
            if (19000 <= f.ID && f.ID <= 19999)
                throw new ProtoFormatException("Can't use reserved field ID 19000-19999", tr);
            if (f.ID > (1 << 29) - 1)
                throw new ProtoFormatException("Maximum field id is 2^29 - 1", tr);

            //Add Field to message
            m.Fields.Add(f.ID, f);
            
            //Determine if extra options
            string extra = tr.ReadNext();
            if (extra == ";")
                return true;
            
            //Field options
            if (extra != "[")
                throw new ProtoFormatException("Expected: [ got " + extra, tr);
            
            ParseFieldOptions(tr, f);
            return true;
        }

        static void ParseFieldOptions(TokenReader tr, Field f)
        {
            while (true)
            {
                string key = tr.ReadNext();
                tr.ReadNextOrThrow("=");
                string val = tr.ReadNext();

                ParseFieldOption(key, val, f);
                string optionSep = tr.ReadNext();
                if (optionSep == "]")
                    break;
                if (optionSep == ",")
                    continue;
                throw new ProtoFormatException(@"Expected "","" or ""]"" got " + tr.NextCharacter, tr);
            }
            tr.ReadNextOrThrow(";");
        }

        static void ParseFieldOption(string key, string val, Field f)
        {
            key = key.Trim('(', ')');
            switch (key)
            {
                case "default":
                    f.OptionDefault = val;
                    break;
                case "packed":
                    f.OptionPacked = Boolean.Parse(val);
                    break;
                case "deprecated":
                    f.OptionDeprecated = Boolean.Parse(val);
                    break;
                case "field_external":
                    f.OptionExternal = Boolean.Parse(val);
                    break;
                case "readonly":
                    f.OptionReadOnly = Boolean.Parse(val);
                    break;
                case "field_access":
                    f.OptionAccess = val.ToLowerInvariant();
                    break;
                case "codetype":
                    f.OptionCodeType = val;
                    break;

                default:
                    Console.WriteLine("Warning: Unknown field option: " + key);
                    break;
            }
        }
        
        /// <summary>
        /// File or Message options
        /// </summary>
        static void ParseOption(TokenReader tr, ProtoMessage m)
        {
            //Read name
            string key = tr.ReadNext();
            if (tr.ReadNext() != "=")
                throw new ProtoFormatException("Expected: = got " + tr.NextCharacter, tr);
            //Read value
            string value = tr.ReadNext();
            if (tr.ReadNext() != ";")
                throw new ProtoFormatException("Expected: ; got " + tr.NextCharacter, tr);
            
            //null = ignore option
            if (m == null)
                return;
            key = key.Trim('(', ')');

            switch (key)
            {
                case "namespace":
                    m.OptionNamespace = value;
                    break;
                case "type":
                    m.OptionType = value.ToLowerInvariant();
                    break;
                case "access":
                    m.OptionAccess = value.ToLowerInvariant();
                    break;
                case "external":
                    m.OptionExternal = Boolean.Parse(value);
                    break;
                case "preserveunknown":
                    m.OptionPreserveUnknown = Boolean.Parse(value);
                    break;
                case "triggers":
                    m.OptionTriggers = Boolean.Parse(value);
                    break;
                default:
                    Console.WriteLine("Warning: Unknown option: " + key + " = " + value);
                    break;
            }
        }

        static void ParseEnum(TokenReader tr, ProtoMessage parent, string package)
        {
            ProtoEnum me = new ProtoEnum(parent, package);

            LocalParser.ParseComments(me, lastComment, tr);
            me.ProtoName = tr.ReadNext();

            parent.Enums.Add(me.ProtoName, me); //must be after .ProtoName is read

            if (tr.ReadNext() != "{")
                throw new ProtoFormatException("Expected: {", tr);
            
            while (true)
            {
                string name = tr.ReadNext();

                if (ParseComment(name))
                    continue;
                
                if (name == "}")
                    return;
                
                //Ignore options
                if (name == "option")
                {
                    ParseOption(tr, null);
                    lastComment.Clear();
                    continue;
                }

                ParseEnumValue(tr, me, name);
            }
            
        }

        static void ParseEnumValue(TokenReader tr, ProtoEnum parent, string name)
        {
            if (tr.ReadNext() != "=")
                throw new ProtoFormatException("Expected: =", tr);

            int id = int.Parse(tr.ReadNext());

            var value = new ProtoEnumValue(name, id, lastComment);
            parent.Enums.Add(value);

            string extra = tr.ReadNext();

            if (extra == ";")
                return;

            if (extra != "[")
                throw new ProtoFormatException("Expected: ; or [", tr);

            ParseEnumValueOptions(tr, value);
        }

        static void ParseEnumValueOptions(TokenReader tr, ProtoEnumValue evalue)
        {
            while (true)
            {
                string key = tr.ReadNext();
                tr.ReadNextOrThrow("=");
                string val = tr.ReadNext();

                ParseEnumValueOptions(key, val, evalue);
                string optionSep = tr.ReadNext();
                if (optionSep == "]")
                    break;
                if (optionSep == ",")
                    continue;
                throw new ProtoFormatException(@"Expected "","" or ""]"" got " + tr.NextCharacter, tr);
            }
            tr.ReadNextOrThrow(";");
        }

        static void ParseEnumValueOptions(string key, string val, ProtoEnumValue f)
        {
            //TODO
        }

        static void ParseExtensions(TokenReader tr, ProtoMessage m)
        {
            //extensions 100 to max;
            tr.ReadNext(); //100
            tr.ReadNextOrThrow("to");
            tr.ReadNext(); //number or max
            tr.ReadNextOrThrow(";");
        }
    }
}

