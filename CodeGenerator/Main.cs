using System;
using System.IO;
using System.Threading;

namespace ProtocolBuffers
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage:\n\tCodeGenerator.exe path-to.proto [path-to-second.proto [...]] [output.cs]");
                Console.Error.WriteLine("Local settings(.csproto) files will be included automatically for mathcing .proto names.");
                return -1;
            }

            ProtoCollection collection = new ProtoCollection();
            string outputPath = null;

            int argIndex = 0;
            while (argIndex < args.Length)
            {
                string protoPath = Path.GetFullPath(args [argIndex]);
                string protoBase = Path.Combine(
                        Path.GetDirectoryName(protoPath),
                        Path.GetFileNameWithoutExtension(protoPath));
                argIndex += 1;

                //First .proto filename is used as output unless specified later
                if (argIndex == 1)
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
                    ProtoCollection proto = new ProtoCollection();

                    //Parse .proto
                    Console.WriteLine("Parsing " + protoPath);
                    ProtoParser.Parse(protoPath, proto);
                
                    //Parse .csproto
                    if (File.Exists(protoBase + ".csproto"))
                    {
                        Console.WriteLine("Parsing " + protoBase + ".csproto");
                        CsProtoParser.Parse(protoBase + ".csproto", proto);
                    }

                    //Save .csproto
                    CsProtoWriter.Save(protoBase + ".csproto", proto);

                    collection.Merge(proto);
                }
#if !DEBUG
                catch (ProtoFormatException pfe)
                {
                    Console.WriteLine("Format error in " + protoPath);
                    if (pfe.CsProto != null)
                        Console.WriteLine(" at line " + pfe.CsProto.Line);
                    Console.Write(pfe.Message);
                    return -1;
                }
#else
                finally {}
#endif
            }

            Console.WriteLine(collection);

            //Interpret and reformat
            try
            {
                ProtoPrepare.Prepare(collection);
            }
            catch (ProtoFormatException pfe)
            {
                Console.WriteLine("Error in preparation:");
                Console.WriteLine("\t" + pfe.Message);
                return -1;
            }

            //Generate code
            ProtoCode.Save(collection, outputPath);
            Console.WriteLine("Saved: " + outputPath);
            return 0;
        }
    }
}
