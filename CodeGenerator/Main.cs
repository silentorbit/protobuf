using System;
using System.IO;
using System.Threading;

namespace ProtocolBuffers
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 3) {
				Console.Error.WriteLine ("Usage:\n\tCodeGenerator.exe <path-to.proto> <namespace> <output.cs>");
				return;						
			}
			
			string protoPath = Path.GetFullPath (args [0]);
			string codeNamespace = args [1];
			string codePath = Path.GetFullPath (args [2]);
			
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
				
			//Generate code
			Console.WriteLine ("Generating code");
			CodeGenerator.Save (proto, codeNamespace, codePath);
			Console.WriteLine ("Saved: " + codePath);
		}
	}
}
