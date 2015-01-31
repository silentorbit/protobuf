using System;
using System.IO;
using System.Threading;
using SilentOrbit.Code;

namespace SilentOrbit.ProtocolBuffers
{
    public class Builder
    {
        public static int Main(string[] args)
        {
            var options = Options.Parse(args);
            if (options == null)
                return -1;
            try
            {
                Build(options);
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static void Build(Options options)
        {
            ProtoCollection collection = new ProtoCollection();

            foreach (string protoPath in options.InputProto)
            {
                try
                {
                    Console.WriteLine("Parsing " + protoPath);

                    var proto = new ProtoCollection();
                    ProtoParser.Parse(protoPath, proto);
                    collection.Merge(proto);
                }
                catch (ProtoFormatException pfe)
                {
                    Console.WriteLine();
                    Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
                    throw;
                }
            }

            Console.WriteLine(collection);

            //Interpret and reformat
            try
            {
                var pp = new ProtoPrepare(options);
                pp.Prepare(collection);
            }
            catch (ProtoFormatException pfe)
            {
                Console.WriteLine();
                Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
                throw;
            }

            //Generate code
            ProtoCode.Save(collection, options);
            Console.WriteLine("Saved: " + options.OutputPath);
        }
    }
}
