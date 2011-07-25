using System;
using ExampleNamespace;
using System.IO;
using ProtoBuf;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			
			TestFeatures ();
			
			TestPersonWire ();
			
			if (total)
				Console.WriteLine ("All test succeeded");
			else
				Console.WriteLine ("Some or all tests failed");
		}
		
		/// <summary>
		/// Test wire format of the person example against protobuf-net - another c# protocol buffers library
		/// </summary>
		static void TestPersonWire ()
		{
			Person p1 = new Person ();
			p1.Name = "Alice";
			p1.Id = 17532;
			p1.Email = "alice@silentorbit.com";
			p1.Phone.Add (new Person.PhoneNumber (){ Type = Person.PhoneType.MOBILE, Number = "+46 11111111111"});
			p1.Phone.Add (new Person.PhoneNumber (){ Type = Person.PhoneType.HOME, Number = "+46 777777777"});
			MemoryStream ms1 = new MemoryStream ();
			Person.Write (ms1, p1);
			
			MemoryStream ms2 = new MemoryStream (ms1.ToArray ());
			NetPerson p2 = Serializer.Deserialize<NetPerson> (ms2);

			//Test
			Test ("12 Name", p1.Name == p2.Name);
			Test ("12 Id", p1.Id == p2.Id);
			Test ("12 Email", p1.Email == p2.Email);
			Test ("12 Phone", p1.Phone.Count == p2.Phone.Count);
			Test ("12 Phone[0]", p1.Phone [0].Number == p2.Phone [0].Number);
			Test ("12 Phone[0]", (int)p1.Phone [0].Type == (int)p2.Phone [0].Type);
			Test ("12 Phone[1]", p1.Phone [1].Number == p2.Phone [1].Number);
			//Disabled test since missing data should return the default value(HOME).
			//Test ("12 Phone[1]", (int)p1.Phone [1].Type == (int)p2.Phone [1].Type);
			
			//Correct invalid data for the next test
			p2.Phone [1].Type = Person.PhoneType.HOME;
			
			MemoryStream ms3 = new MemoryStream ();
			Serializer.Serialize (ms3, p2);

			//Test wire data
			byte[] b1 = ms1.ToArray ();
			byte[] b3 = ms3.ToArray ();
			Test ("WireLength", b1.Length == b3.Length);
			if (b1.Length == b3.Length) {
				for (int n = 0; n < b1.Length; n++)
					if (b1 [n] != b3 [n])
						Test ("Wire" + n, b1 [n] == b3 [n]);
			} else {
				Console.WriteLine (BitConverter.ToString (b1));
				Console.WriteLine ();
				Console.WriteLine (BitConverter.ToString (b3));
			}
			
			MemoryStream ms4 = new MemoryStream (ms3.ToArray ());
			Person p4 = Person.Read (ms4);
			
			//Test			
			Test ("14 Name", p1.Name == p4.Name);
			Test ("14 Id", p1.Id == p4.Id);
			Test ("14 Email", p1.Email == p4.Email);
			Test ("14 Phone", p1.Phone.Count == p4.Phone.Count);
			Test ("14 Phone[0]", p1.Phone [0].Number == p4.Phone [0].Number);
			Test ("14 Phone[0]", p1.Phone [0].Type == p4.Phone [0].Type);
			Test ("14 Phone[1]", p1.Phone [1].Number == p4.Phone [1].Number);
			Test ("14 Phone[1]", p1.Phone [1].Type == p4.Phone [1].Type);
		}
		
		[ProtoContract]
		class NetPerson
		{
			[ProtoMember(1)]
			public string Name;

			[ProtoMember(2)]
			public int Id { get; set; }

			[ProtoMember(3)]
			public string Email { get; set; }

			[ProtoMember(4)]
			public List<NetPhoneNumber> Phone { get; set; }
		
			[ProtoContract]
			public class NetPhoneNumber : Person.IPhoneNumber
			{
				[ProtoMember(1)]
				public string Number { get; set; }

				[ProtoMember(2)]
				[DefaultValue(Person.PhoneType.HOME)]
				public Person.PhoneType Type { get; set; }
			}
		}
		
		
		/// <summary>
		/// This is a simple test to trigger most functionality of the generated code.
		/// </summary>
		static void TestFeatures ()
		{
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
			
			Console.WriteLine ("Wire bytes: " + ms.Length);
			
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

		static bool total = true;
		
		private static void Test (string message, bool result)
		{
			if (result == false)
				total = false;
			
			Console.WriteLine (message + ": " + (result ? "OK" : "Fail!"));
		}
		
	}
}
