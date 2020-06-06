using System;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Options.TryParse(args, (options) => Build(options));
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }

        public static void Build(Options options)
        {
            var parser = new FileParser();
            var collection = parser.Import(options.InputProto);

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
