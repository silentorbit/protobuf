using System;
using System.IO;
using System.Text;

namespace ProtocolBuffers
{
    static class CsProtoWriter
    {
        public static void Save(string path, ProtoCollection proto)
        {
            using (TextWriter writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                writer.WriteLine("// Custom features based on " + Path.GetFileNameWithoutExtension(path) + ".proto");
                writer.WriteLine("// This file will be parsed and rewritten when CodeGenerator is run");
                writer.WriteLine("// Comments are lost");
                writer.WriteLine("");

                WriteMessages(proto, writer);
            }
        }

        static void WriteMessages(ProtoMessage message, TextWriter w)
        {
            foreach (ProtoMessage m in message.Messages.Values)
            {
                w.WriteLine("message " + m.FullProtoName);
                //Options
                w.WriteLine("\taccess = " + m.OptionAccess + "\t//public, internal");
                if (m.OptionNamespace == null)
                    w.WriteLine("\t//namespace = " + m.Package);
                else
                    w.WriteLine("\tnamespace = " + m.OptionNamespace);
                if (m.OptionType == null || m.OptionType == "class")
                    w.WriteLine("\t//type = class\t//class, struct or interface");
                else
                    w.WriteLine("\ttype = " + m.OptionType + "\t//class, struct or interface");
                w.WriteLine("\t" + (m.OptionPreserveUnknown ? "" : "//") + "preserveunknown");
                w.WriteLine("\t" + (m.OptionTriggers ? "" : "//") + "triggers");
                w.WriteLine("\t" + (m.OptionExternal ? "" : "//") + "external");
                w.WriteLine("{");
                //Fields
                foreach (Field f in m.Fields.Values)
                {
                    w.WriteLine("\t" + f.ProtoName);
                    w.WriteLine("\t\taccess = " + f.OptionAccess + "\t//public, internal, protected or private");
                    if (f.OptionCodeType == null)
                        w.WriteLine("\t\t//codetype = DateTime or TimeSpan");
                    else
                        w.WriteLine("\t\tcodetype = " + f.OptionCodeType);
                    w.WriteLine("\t\t" + (f.OptionExternal ? "" : "//") + "external");
                    w.WriteLine("\t\t" + (f.OptionReadOnly ? "" : "//") + "readonly");
                    w.WriteLine();
                }
                w.WriteLine("}");
                w.WriteLine();

                WriteMessages(m, w);
            }
        }

    }
}

