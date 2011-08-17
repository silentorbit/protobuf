using System;
using System.IO;

namespace ProtocolBuffers
{
	public static partial class ProtocolParser
	{
		/// <summary>
		/// Read TimeSpan from Ticks in a int64.
		/// </summary>
		public static TimeSpan ReadTimeSpan (Stream stream)
		{
			return 
		}
		
		/// <summary>
		/// Write TimeSpan Ticks in a int64.
		/// </summary>
		public static void WriteTimeSpan (Stream stream, TimeSpan time)
		{
			WriteUInt64 (stream, (ulong)time.Ticks);
		}
		
		
		/// <summary>
		/// Read DateTime from Ticks in a int64.
		/// </summary>
		public static DateTime ReadDateTime (Stream stream)
		{
			return new DateTime((long)ReadUInt64 (stream));
		}
		
		/// <summary>
		/// Write DateTime Ticks in a int64.
		/// </summary>
		public static void WriteDateTime (Stream stream, DateTime time)
		{
			WriteUInt64 (stream, (ulong)time.Ticks);
		}
	}
}

