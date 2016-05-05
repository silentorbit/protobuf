using System;
using System.IO;
using System.Threading;
using SilentOrbit.Code;
using System.Collections.Generic;

namespace SilentOrbit.ProtocolBuffers
{
    public class Builder
    {
        public static int Main(string[] args)
        {
            var options = Options.Parse(args);
#if BUILD_TESTS
            string testBuildArgs = @"--fix-nameclash --ctor --utc --skip-default ..\..\..\TestProgram\ProtoSpec\ImportAll.proto --output ..\..\..\TestProgram\Generated\Generated.cs";
            options = Options.Parse(testBuildArgs.Split(' '));
#else
#endif
            if (options == null)
                return -1;

            try
            {
                Build(options);
#if BUILD_TESTS
                Console.WriteLine("\nSUCCESS, press a key to close...");
                Console.ReadKey();
#endif
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
