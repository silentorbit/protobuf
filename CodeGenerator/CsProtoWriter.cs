using System;
using System.IO;
using System.Text;

namespace ProtocolBuffers
{
    static class CsProtoWriter
    {
        public static void Save(string path, ProtoCollection proto)
        {
            using (CodeWriter writer = new CodeWriter(path))
            {
                writer.Comment("Custom features based on " + Path.GetFileNameWithoutExtension(path) + ".proto");
                writer.Comment("This file will be parsed and rewritten when CodeGenerator is run");
                writer.Comment("Comments are lost");
                writer.WriteLine();

                WriteMessages(proto, writer);
            }
        }

        static void WriteMessages(ProtoMessage message, CodeWriter w)
        {
            foreach (ProtoMessage m in message.Messages.Values)
            {
                w.WriteLine("message " + m.FullProtoName);
                //Options
                w.Indent();
                w.WriteLine("access = " + m.OptionAccess + "\t//public or internal");
                if (m.OptionNamespace == null)
                    w.Comment("namespace = " + m.Package);
                else
                    w.WriteLine("namespace = " + m.OptionNamespace + " // Default from .proto: " + m.Package);
                if (m.OptionType == null || m.OptionType == "class")
                    w.Comment("type = class\t//class, struct or interface");
                else
                    w.WriteLine("type = " + m.OptionType + "\t//class, struct or interface");
                w.WriteLine((m.OptionPreserveUnknown ? "" : "//") + "preserveunknown");
                w.WriteLine((m.OptionTriggers ? "" : "//") + "triggers");
                w.WriteLine((m.OptionExternal ? "" : "//") + "external");
                w.Dedent();
                w.Bracket();
                //Fields
                foreach (Field f in m.Fields.Values)
                {
                    w.Comment(f.Comments);
                    w.Comment(f.Rule + " " + f.ProtoTypeName + " " + f.ProtoName + " = " + f.ID);
                    w.WriteLine(f.ProtoName);
                    w.Indent();
                    w.WriteLine("access = " + f.OptionAccess + "\t//public, internal, protected or private");
                    if (f.OptionCodeType == null)
                        w.Comment("codetype = DateTime or TimeSpan");
                    else
                        w.WriteLine("codetype = " + f.OptionCodeType);
                    w.WriteLine((f.OptionExternal ? "" : "//") + "external");
                    w.WriteLine((f.OptionReadOnly ? "" : "//") + "readonly");
                    w.WriteLine();
                    w.Dedent();
                }
                w.EndBracket();
                w.WriteLine();

                WriteMessages(m, w);
            }
        }

    }
}

