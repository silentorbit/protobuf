using System;
using System.Collections.Generic;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
    class FileParser
    {
        /// <summary>
        /// Paths that has already been imported
        /// </summary>
        readonly Dictionary<string, ProtoCollection> imported = new Dictionary<string, ProtoCollection>();

        //Non public import, only include further public imports from here
        readonly List<string> toImport = new List<string>();

        public ProtoCollection Import(IEnumerable<string> inputProto)
        {
            var collection = new ProtoCollection();

            //Files from command line arguments, all import statements will be followed
            var basePath = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()), "dummy"); 
            foreach (string rawPath in inputProto)
            {
                string protoPath = GetFullPath(basePath, rawPath);
                var proto = Import(protoPath);
                collection.Merge(proto);

                //Include non public imports for the first level
                foreach (var path in proto.Import)
                    toImport.Add(GetFullPath(protoPath, path));
            }

            //Read imported files, nested
            while (toImport.Count != 0)
            {
                var path = toImport[0];
                toImport.RemoveAt(0);

                var c = Import(path);
                if (c != null)
                    collection.Merge(c);
            }
            return collection;
        }

        ProtoCollection Import(string protoPath)
        {
            if (imported.ContainsKey(protoPath))
                return null; //Already imported
                
            var proto = new ProtoCollection();

            try
            {
                Console.WriteLine("Parsing " + protoPath);
                ProtoParser.Parse(protoPath, proto);
            }
            catch (ProtoFormatException pfe)
            {
                Console.WriteLine();
                Console.WriteLine(pfe.SourcePath.Path + "(" + pfe.SourcePath.Line + "," + pfe.SourcePath.Column + "): error CS001: " + pfe.Message);
                throw;
            }

            foreach (var path in proto.ImportPublic)
                toImport.Add(GetFullPath(protoPath, path));

            imported.Add(protoPath, proto);

            //Mark imported
            proto.MarkImported();

            return proto;
        }

        static string GetFullPath(string baseProtoPath, string importPath)
        {
            if (Path.IsPathRooted(importPath))
                return Path.GetFullPath(importPath);

            var dir = Path.GetDirectoryName(baseProtoPath);
            return Path.GetFullPath(Path.Combine(dir, importPath));
        }
    }
}

