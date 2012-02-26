using System;
using System.IO;
using System.Threading;

namespace ProtocolBuffers
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length < 1) {
				Console.Error.WriteLine ("Usage:\n\tCodeGenerator.exe path-to.proto [output.cs]");
				return;						
			}
			
			//Currently the format is CodeGenerator.exe file1.proto [file1otherpath.cs] file2.proto [file2otherpath.cs]
			//Output paths must end in .cs
			
			int argIndex = 0;
			while (argIndex < args.Length) {
				string protoPath = Path.GetFullPath (args [argIndex]);
				argIndex += 1;
				
				if (File.Exists (protoPath) == false) {
					Console.Error.WriteLine ("File not found: " + protoPath);
					if (args [argIndex].EndsWith (".cs"))
						argIndex += 1;
					continue;
				}

				//Parse proto
				Console.WriteLine ("Parsing " + protoPath);
				Proto proto = ProtoParser.Parse (protoPath);
				if (proto == null) {
					if (args [argIndex].EndsWith (".cs"))
						argIndex += 1;
					continue;
				}
				Console.WriteLine (proto);

				//Interpret and reformat
				ProtoPrepare.Prepare (proto);
			
				string codePath;
				if (argIndex < args.Length && args [argIndex].EndsWith (".cs")) {
					codePath = Path.GetFullPath (args [argIndex]);
					argIndex += 1;					
				} else {
					string ext = Path.GetExtension (protoPath);
					codePath = protoPath.Substring (0, protoPath.Length - ext.Length) + ".cs";
				}

				//Generate code
				ProtoCode.Save (proto, new MessageCode (), codePath);
				Console.WriteLine ("Saved: " + codePath);
			}
		}
	}
}
