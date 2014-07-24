using System;
using System.IO;

namespace SilentOrbit.Code
{
    public static class Name
    {
        public static string ToCamelCase(string name)
        {
            if (name == "")
                throw new ArgumentException("Empty name");

            string csname = "";

            foreach (string part in name.Split('.'))
            {
                if (csname != "")
                    csname += ".";
                csname += ToCamelCaseSingleName(part);
            }

            return csname;
        }

        static string ToCamelCaseSingleName(string name)
        {
            string csname = "";

            foreach (string part in name.Split('_', '-'))
            {
                if (part.Length == 0)
                    csname += "_";
                else
                    csname += part.Substring(0, 1).ToUpperInvariant() + part.Substring(1);
            }

            return csname;
        }
    }
}

