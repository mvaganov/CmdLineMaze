using System;

namespace SimpleGame {
	/// <summary>
	/// a linear congruential number generator
	/// </summary>
	class Rando {
		static long seed;
		public static void Seed(long num) { seed = num; }
		public static long Next => Math.Abs(seed = seed * 6364136223846793005 + 1442695040888963407);
	}
}
