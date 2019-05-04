using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SilentOrbit.ProtocolBuffers
{
    /// <summary>
    /// Options set using Command Line arguments
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Keep names as written in .proto, otherwise class and field names by default are converted to CamelCase
        /// </summary>
        public bool PreserveNames { get; set; }

        /// <summary>
        /// If a property name is the same as its class name or any subclass the property will be renamed. if the name clash occurs and this flag is not set, an error will occur and the code generation is aborted.
        /// </summary>
        public bool FixNameclash { get; set; }

        /// <summary>
        /// If set generated code will use tabs rather than 4 spaces.
        /// </summary>
        public bool UseTabs { get; set; }

        public IEnumerable<string> InputProto { get; set; }

        /// <summary>
        /// Path to the generated cs files
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Assign the name of the stack implementatino to use for each message type, included options are ThreadSafeStack, ThreadUnsafeStack, ConcurrentBagStack or the full namespace to your own implementation.
        /// </summary>
        public string ExperimentalStack { get; set; }

        /// <summary>
        /// Generate constructors with default values.
        /// </summary>
        public bool GenerateDefaultConstructors { get; set; }

        /// <summary>
        /// Generate nullable primitives for optional fields
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// Exclude code that require .NET 4
        /// </summary>
        public bool Net2 { get; set; }

        /// <summary>
        /// De/serialize DateTime as DateTimeKind.Utc
        /// </summary>
        public bool Utc { get; set; }

        /// <summary>
        /// Add the [Serializable] attribute to generated classes
        /// </summary>
        public bool SerializableAttributes { get; set; }

        /// <summary>
        /// Skip serializing properties having the default value.
        /// </summary>
        public bool SkipSerializeDefault { get; set; }

        /// <summary>
        /// Do not output ProtocolParser.cs
        /// </summary>
        public bool NoProtocolParser { get; set; }

        /// <summary>
        /// Don't generate code from imported .proto files.
        /// </summary>
        public bool NoGenerateImported { get; set; }

    }
}
