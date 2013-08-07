using System;
using System.IO;
using System.Threading;

namespace SilentOrbit.ProtocolBuffers
{
    class MainClass
    {
        public static int Main(string[] args)
        {
            var options = Options.Parse(args);
            if(options == null)
                return -1;

            ProtoCollection collection = new ProtoCollection();

            foreach(string protoPath in options.InputProto)
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
                    return -1;
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
                return -1;
            }

            //Generate code
            ProtoCode.Save(collection, options);
            Console.WriteLine("Saved: " + options.OutputPath);
            return 0;
        }
    }
}
