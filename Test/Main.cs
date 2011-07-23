using System;
using ExampleNamespace;
using System.IO;

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
		
			///This is a simple test to trigger most functionality of the generated code.
		
			IMyMessageV2 mm = new MyMessageV2 ();
			mm.FieldA = 1;
			mm.FieldB = 2.2;
			mm.FieldC = 3.3f;
			mm.FieldD = -4;
			mm.FieldE = -5;
			mm.FieldF = 6;
			mm.FieldG = 7;
			mm.FieldH = -8;
			mm.FieldI = -9;
			mm.FieldJ = 10;
			mm.FieldK = 11;
			mm.FieldL = -12;
			mm.FieldM = -13;
			mm.FieldN = true;
			mm.FieldO = "test1";
			mm.FieldP = new byte[] { 0, 1, 2, 3, 4 };
			mm.FieldQ = MyMessageV2.MyEnum.ETest1;
			mm.FieldR = MyMessageV2.MyEnum.ETest3;
			mm.FieldS.Add (20);
			mm.FieldS.Add (120);
			mm.FieldS.Add (1120);
			mm.FieldT.Add (21);
			mm.FieldT.Add (121);
			mm.FieldT.Add (1121);
			mm.FieldU = new TheirMessage (){ FieldA = 22};
			mm.FieldV.Add (new TheirMessage (){ FieldA = 23});
			mm.FieldV.Add (new TheirMessage (){ FieldA = 123});
			mm.FieldV.Add (new TheirMessage (){ FieldA = 1123});
			
			//Write
			MemoryStream ms = new MemoryStream ();
			MyMessageV2.Write (ms, mm);

			Console.WriteLine("Wire bytes: " + ms.Length);
			
			//Read
			MemoryStream msread = new MemoryStream (ms.ToArray ());
			IMyMessageV2 mo = MyMessageV2.Read (msread);
			
			//Verify
			Test ("FieldA", mm.FieldA == mo.FieldA);
			Test ("FieldB", mm.FieldB == mo.FieldB);
			Test ("FieldC", mm.FieldC == mo.FieldC);
			Test ("FieldD", mm.FieldD == mo.FieldD);
			Test ("FieldE", mm.FieldE == mo.FieldE);
			Test ("FieldF", mm.FieldF == mo.FieldF);
			Test ("FieldG", mm.FieldG == mo.FieldG);
			Test ("FieldH", mm.FieldH == mo.FieldH);
			Test ("FieldI", mm.FieldI == mo.FieldI);
			Test ("FieldJ", mm.FieldJ == mo.FieldJ);
			Test ("FieldK", mm.FieldK == mo.FieldK);
			Test ("FieldL", mm.FieldL == mo.FieldL);
			Test ("FieldM", mm.FieldM == mo.FieldM);
			Test ("FieldN", mm.FieldN == mo.FieldN);
			Test ("FieldO", mm.FieldO == mo.FieldO);
			Test ("FieldP.Length", mm.FieldP.Length == mo.FieldP.Length);
			for (int n = 0; n < mm.FieldP.Length; n++)
				Test ("FieldP[" + n + "]", mm.FieldP [n] == mo.FieldP [n]);
			Test ("FieldQ", mm.FieldQ == mo.FieldQ);
			Test ("FieldR", mm.FieldR == mo.FieldR);
			Test ("FieldS.Count", mm.FieldS.Count == mo.FieldS.Count);
			Test ("FieldS 0", mm.FieldS [0] == mo.FieldS [0]);
			Test ("FieldS 1", mm.FieldS [1] == mo.FieldS [1]);
			Test ("FieldS 2", mm.FieldS [2] == mo.FieldS [2]);
			Test ("FieldT.Count", mm.FieldT.Count == mo.FieldT.Count);
			Test ("FieldT 0", mm.FieldT [0] == mo.FieldT [0]);
			Test ("FieldT 1", mm.FieldT [1] == mo.FieldT [1]);
			Test ("FieldT 2", mm.FieldT [2] == mo.FieldT [2]);
			Test ("FieldU", mm.FieldU.FieldA == mo.FieldU.FieldA);
			Test ("FieldV.Count", mm.FieldV.Count == mo.FieldV.Count);
			Test ("FieldV 0", mm.FieldV [0].FieldA == mo.FieldV [0].FieldA);
			Test ("FieldV 1", mm.FieldV [1].FieldA == mo.FieldV [1].FieldA);
			Test ("FieldV 2", mm.FieldV [2].FieldA == mo.FieldV [2].FieldA);
			
			//Read by older version
			MemoryStream msread1 = new MemoryStream (ms.ToArray ());
			IMyMessageV1 m1 = MyMessageV1.Read (msread1);
			
			Console.WriteLine ("Version 1");
			Test ("FieldA", mm.FieldA == m1.FieldA);
		}
		
		private static void Test (string message, bool result)
		{
			Console.WriteLine (message + ": " + (result ? "OK" : "Fail!"));
		}
		
	}
}
