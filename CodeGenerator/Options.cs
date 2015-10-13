using System;
using CommandLine;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using CommandLine.Text;

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

            var inputs = new List<string>(options.InputProto);
            options.InputProto = inputs;
            for (int n = 0; n < inputs.Count; n++)
            {
                inputs[n] = Path.GetFullPath(inputs[n]);
                if (File.Exists(inputs[n]) == false)
                {
                    Console.Error.WriteLine("File not found: " + inputs[n]);
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

        public string GetUsage()
        {
            var help = new HelpText {
                //Heading = new HeadingInfo("ProtoBuf Code Generator", "1.0."),
                //Copyright = new CopyrightInfo("Peter Hultqvist", 2015),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("Usage: CodeGenerator.exe [input-files.proto] --output output-file.cs");
            help.AddOptions(this);
            return help;
        }
    }
}

