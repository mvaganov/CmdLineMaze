using System;

namespace CustomHashTable {
	class Program {
		static void Main(string[] args) {
			HTable<string, int> stats = new HTable<string, int>(HashFunctions.GetDeterministicHashCode) {
				["str"] = 10,
				["dex"] = 18,
				["con"] = 12,
				["int"] = 10,
				["wis"] = 10,
				["cha"] = 15,
			};
			Console.WriteLine(stats.ToDebugString());
			HTable<string, int>.KV[] kvs = new HTable<string, int>.KV[] {
				new HTable<string, int>.KV{key="str",val=10},
				new HTable<string, int>.KV{key="dex",val=11},
				new HTable<string, int>.KV{key="con",val=12},
				new HTable<string, int>.KV{key="int",val=13},
				new HTable<string, int>.KV{key="wis",val=14},
				new HTable<string, int>.KV{key="cha",val=15},
			};
			for(int i = 0; i < kvs.Length; ++i) {
				stats[kvs[i].key] = kvs[i].val;
			}
			Console.WriteLine("\n"+stats.ToDebugString()+ "\n");
			foreach (var kvp in stats) {
				Console.WriteLine(kvp.Key + ": " + kvp.Value);
			}
			Console.WriteLine("done!");
		}
	}
}
