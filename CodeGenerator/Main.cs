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
                Console.Error.WriteLine("Usage:\n\tCodeGenerator.exe [--preserve-names] [--use-tabs] path-to.proto [path-to-second.proto [...]] [output.cs]");
                return -1;
            }

            ProtoCollection collection = new ProtoCollection();
            string outputPath = null;

            int argIndex = 0;

            while (args.Length > argIndex && args [argIndex].StartsWith("--"))
            {
                switch (args[argIndex])
                {
                    case "--preserve-names":
                        ProtoPrepare.ConvertToCamelCase = false;
                        break;
                    case ProtoPrepare.FixNameclashArgument:
                        ProtoPrepare.FixNameclash = true;
                        break;
                    case "--use-tabs":
                        CodeWriter.IndentPrefix = "\t";
                        break;
                    default:
                        Console.Error.WriteLine("Unknown option: " + args[argIndex]);
                        return -1;
                }
                argIndex++;
            }

            while (argIndex < args.Length)
            {
                string protoPath = Path.GetFullPath(args[argIndex]);
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

                //Handle last argument as the output path
                //Filename is taken from first .proto argument
                if (argIndex == args.Length && protoPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    Directory.CreateDirectory(protoPath);

                    // Replace the original output directory with the custom one
                    outputPath = Path.Combine(protoPath, Path.GetFileName(outputPath));
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
