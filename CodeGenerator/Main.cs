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
			string protoPath = Path.GetFullPath (args [0]);
			
			if (File.Exists (protoPath) == false) {
				Console.Error.WriteLine ("File not found: " + protoPath);
				return;						
			}

			//Parse proto
			Console.WriteLine ("Parsing " + protoPath);
			Proto proto = ProtoParser.Parse (protoPath);
			if (proto == null)
				return;
			Console.WriteLine (proto);

			//Interpret and reformat
			ProtoPrepare.Prepare (proto);
			
			string codePath;
			if (args.Length < 2) {
				string ext = Path.GetExtension (protoPath);
				codePath = protoPath.Substring (0, protoPath.Length - ext.Length) + ".cs";
			} else
				codePath = Path.GetFullPath (args [1]);

			//Generate code
			Console.WriteLine ("Generating code");
			ProtoCode.Save (proto, new MessageCode (), codePath);
			Console.WriteLine ("Saved: " + codePath);
		}
	}
}
