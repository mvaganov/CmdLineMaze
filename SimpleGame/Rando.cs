using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGame {
	/// <summary>
	/// a linear congruential number generator
	/// </summary>
	class Rando {
		static long seed;
		public static void Seed(long num) { seed = num; }
		public static long Next => seed = seed * 6364136223846793005 + 1442695040888963407;
	}
}
