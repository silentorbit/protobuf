using System;
using CommandLine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CommandLine.Text;
using System.Reflection;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// Options set using Command Line arguments
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Show the help
        /// </summary>
        [Option('h', "help", HelpText = "Show this help")]
        public bool ShowHelp { get; set; }

        /// <summary>
        /// Convert message/class and field/propery names to CamelCase
        /// </summary>
        [Option("preserve-names", HelpText = "Keep names as written in .proto, otherwise class and field names by default are converted to CamelCase")]
        public bool PreserveNames { get; set; }

        /// <summary>
        /// If false, an error will occur.
        /// </summary>
        [Option("fix-nameclash", HelpText = "If a property name is the same as its class name or any subclass the property will be renamed. if the name clash occurs and this flag is not set, an error will occur and the code generation is aborted.")]
        public bool FixNameclash { get; set; }

        /// <summary>
        /// Generated code indent using tabs
        /// </summary>
        [Option('t', "use-tabs", HelpText = "If set generated code will use tabs rather than 4 spaces.")]
        public bool UseTabs { get; set; }

        [Value(0, Required = true)]
        public IEnumerable<string> InputProto { get; set; }

        /// <summary>
        /// Path to the generated cs files
        /// </summary>
        [Option('o', "output", Required = false, HelpText = "Path to the generated .cs file.")]
        public string OutputPath { get; set; }

        /// <summary>
        /// Use experimental stack per message type
        /// </summary>
        [Option("experimental-message-stack", HelpText = "Assign the name of the stack implementatino to use for each message type, included options are ThreadSafeStack, ThreadUnsafeStack, ConcurrentBagStack or the full namespace to your own implementation.")]
        public string ExperimentalStack { get; set; }

        /// <summary>
        /// If set default constructors will be generated for each message
        /// </summary>
        [Option("ctor", HelpText = "Generate constructors with default values.")]
        public bool GenerateDefaultConstructors { get; set; }

        /// <summary>
        /// Use Nullable&lt;&gt; for optional fields
        /// </summary>
        [Option("nullable", Required = false, HelpText = "Generate nullable primitives for optional fields")]
        public bool Nullable { get; set; }

        /// <summary>
        /// Exclude .NET 4 code
        /// </summary>
        [Option("net2", Required = false, HelpText = "Exclude code that require .NET 4")]
        public bool Net2 { get; set; }

        /// <summary>
        /// De/serialize DateTime as UTC only
        /// </summary>
        [Option("utc", Required = false, HelpText = "De/serialize DateTime as DateTimeKind.Utc")]
        public bool Utc { get; set; }

        /// <summary>
        /// Add the [Serializable] attribute to generated classes
        /// </summary>
        [Option("serializable", Required = false, HelpText = "Add the [Serializable] attribute to generated classes")]
        public bool SerializableAttributes { get; set; }

        /// <summary>
        /// Skip serializing properties having the default value.
        /// </summary>
        [Option("skip-default", Required = false, HelpText = "Skip serializing properties having the default value.")]
        public bool SkipSerializeDefault { get; set; }

        /// <summary>
        /// Do not output ProtocolParser.cs
        /// </summary>
        [Option("no-protocolparser", Required = false, HelpText = "Don't output ProtocolParser.cs")]
        public bool NoProtocolParser { get; set; }

        /// <summary>
        /// Don't generate code from imported .proto files.
        /// </summary>
        [Option("no-generate-imported", Required = false, HelpText = "Don't generate code from imported .proto files.")]
        public bool NoGenerateImported { get; set; }


        public static Options Parse(string[] args)
        {
            var result = Parser.Default.ParseArguments<Options>(args);
            var options = result.Value;
            if (result.Errors.Any())
                return null;

            if (args == null || args.Length == 0 || options.ShowHelp)
            {
                Console.Error.WriteLine(options.GetUsage());
                return null;
            }

            bool error = false;

            //Do any extra option checking/cleanup here
            if (options.InputProto == null)
            {
                Console.Error.WriteLine("Missing input .proto arguments.");
                return null;
            }

            var inputs = ExpandFileWildCard(options.InputProto);
            options.InputProto = inputs;
            foreach (var input in inputs)
            {
                if (File.Exists(input) == false)
                {
                    Console.Error.WriteLine("File not found: " + input);
                    error = true;
                }
            }

            //Backwards compatibility
            string firstPathCs = inputs[0];
            firstPathCs = Path.Combine(
                Path.GetDirectoryName(firstPathCs),
                Path.GetFileNameWithoutExtension(firstPathCs)) + ".cs";

            if (options.OutputPath == null)
            {
                //Use first .proto as base for output
                options.OutputPath = firstPathCs;
                Console.Error.WriteLine("Warning: Please use the new syntax: --output \"" + options.OutputPath + "\"");
            }
            //If output is a directory then the first input filename will be used.
            if (options.OutputPath.EndsWith(Path.DirectorySeparatorChar.ToString()) || Directory.Exists(options.OutputPath))
            {
                Directory.CreateDirectory(options.OutputPath);
                options.OutputPath = Path.Combine(options.OutputPath, Path.GetFileName(firstPathCs));
            }
            options.OutputPath = Path.GetFullPath(options.OutputPath);

            if (options.ExperimentalStack != null && !options.ExperimentalStack.Contains("."))
                options.ExperimentalStack = "global::SilentOrbit.ProtocolBuffers." + options.ExperimentalStack;

            if (error)
                return null;
            else
                return options;
        }

        /// <summary>
        /// Expand wildcards in the filename part of the input file argument.
        /// </summary>
        /// <returns>List of full paths to the files.</returns>
        /// <param name="paths">List of relative paths with possible wildcards in the filename.</param>
        static List<string> ExpandFileWildCard(IEnumerable<string> paths)
        {
            //Thanks to https://stackoverflow.com/a/2819150
            var list = new List<string>();

            foreach (var path in paths)
            {
                var expandedPath = Environment.ExpandEnvironmentVariables(path);

                var dir = Path.GetDirectoryName(expandedPath);
                if (dir.Length == 0)
                    dir = ".";

                var file = Path.GetFileName(expandedPath);

                foreach (var filepath in Directory.GetFiles(dir, file))
                    list.Add(Path.GetFullPath(filepath));
            }

            return list;
        }

        public string GetUsage()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var help = new HelpText
            {
                Heading = new HeadingInfo("ProtoBuf Code Generator", version.ToString()),
                Copyright = new CopyrightInfo("Peter Hultqvist", version.Major),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: CodeGenerator.exe [input-files.proto] --output output-file.cs");
            help.AddOptions(this);
            return help;
        }
    }
}

