using System;
using System.IO;
using System.Threading;

namespace SilentOrbit.ProtocolBuffers
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage:\n\tCodeGenerator.exe [--preserve-names] path-to.proto [path-to-second.proto [...]] [output.cs]");
                return -1;
            }

            ProtoCollection collection = new ProtoCollection();
            string outputPath = null;

            int argIndex = 0;

            if (args.Length > 0 && args [0] == "--preserve-names")
            {
                ProtoPrepare.ConvertToCamelCase = false;
                argIndex++;
            }

            while (argIndex < args.Length)
            {
                string protoPath = Path.GetFullPath(args [argIndex]);
                string protoBase = Path.Combine(
                        Path.GetDirectoryName(protoPath),
                        Path.GetFileNameWithoutExtension(protoPath));
                argIndex += 1;

                //First .proto filename is used as output unless specified later
                if (outputPath == null)
                    outputPath = protoBase + ".cs";
                //Handle last argument as the output .cs path
                if (argIndex == args.Length && protoPath.EndsWith(".cs"))
                {
                    outputPath = protoPath;
                    break;
                }

                if (File.Exists(protoPath) == false)
                {
                    Console.Error.WriteLine("File not found: " + protoPath);
                    return -1;
                }

                try
                {
                    var proto = new ProtoCollection();

                    //Parse .proto
                    Console.WriteLine("Parsing " + protoPath);
                    ProtoParser.Parse(protoPath, proto);
                
                    collection.Merge(proto);
                } catch (ProtoFormatException pfe)
                {
                    Console.WriteLine();
                    Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
                    return -1;
                }
            }

            Console.WriteLine(collection);

            //Interpret and reformat
            try
            {
                ProtoPrepare.Prepare(collection);
            } catch (ProtoFormatException pfe)
            {
                Console.WriteLine();
                Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
                return -1;
            }

            //Generate code
            ProtoCode.Save(collection, outputPath);
            Console.WriteLine("Saved: " + outputPath);
            return 0;
        }
    }
}
