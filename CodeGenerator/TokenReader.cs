using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    class TokenReader
    {
        public readonly string Path;
        readonly string whitespace = " \t\r\n";
        readonly string singletoken = "{}=[];,";
        readonly string text;
        
        public TokenReader(string text, string path)
        {
            this.text = text;           
            this.Path = path;
        }
        
        public string Parsed
        {
            get
            {
                return text.Substring(0, offset);
            }
        }

        public char NextCharacter
        {
            get
            {
                return text [offset];
            }
        }
        
        int offset;
        
        private string GetChar()
        {
            if (offset >= text.Length)
                throw new EndOfStreamException();
            
            char c = text [offset];
            offset += 1;
            return c.ToString();
        }
        
        /// <summary>
        /// Read next token and throw a ProtoFormatException if the token was not the expected one.
        /// </summary>
        public void ReadNextOrThrow(string expect)
        {
            try
            {
                string n = ReadNext();
                if (n != expect)
                    throw new ProtoFormatException("Expected: " + expect + " got " + n, this);
            } catch (EndOfStreamException)
            {
                throw new ProtoFormatException("Expected: " + expect + " got EOF", this);
            }
        }
        
        public string ReadNext()
        {
            string c;   //Character
            
            //Skip whitespace characters
            while (true)
            {
                c = GetChar();
                if (whitespace.Contains(c))
                    continue;
                break;
            }
            
            //Determine token type
            if (singletoken.Contains(c))
                return c.ToString();
        
            //Follow token
            string token = c;
            bool parseString = false;
            bool parseComment = false;
            
            if (token == "/")
                parseComment = true;
            if (token == "\"")
            {
                parseString = true;
                token = "";
            }
            
            while (true)
            {
                c = GetChar();
                if (parseComment)
                {
                    if (c == "\r" || c == "\n")
                        return token;
                } else if (parseString)
                {
                    if (c == "\"")
                        return token;
                } else if (whitespace.Contains(c) || singletoken.Contains(c))
                {
                    offset -= 1;
                    return token;
                }
                
                token += c;
            }
            
        }
    }
}

