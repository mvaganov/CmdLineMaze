//#define EXTRA_DATA_TEST
using System;
using System.Collections.Generic;

namespace CustomHashTable {
	class Program {
		static void Main(string[] args) {
			BasicMap<string, int> bMap = new BasicMap<string, int>();
			SortedMap<string, int> sMap = new SortedMap<string, int>();
			SortedHashedMap<string, int> shMap = new SortedHashedMap<string, int>();
			HTable<string, int> hTable = new HTable<string, int>(HashFunctions.GetDeterministicHashCode, 8);
			KeyValuePair<string,int>[] kvs = new KeyValuePair<string, int>[] {
				new KeyValuePair<string, int>("str",10),
				new KeyValuePair<string, int>("dex",11),
				new KeyValuePair<string, int>("con",12),
				new KeyValuePair<string, int>("int",13),
				new KeyValuePair<string, int>("wis",14),
				new KeyValuePair<string, int>("cha",15),
#if EXTRA_DATA_TEST
				new KeyValuePair<string, int>("Athletics",0),
				new KeyValuePair<string, int>("Acrobatics",1),
				new KeyValuePair<string, int>("SleightOfHand",2),
				new KeyValuePair<string, int>("Stealth",2),
				new KeyValuePair<string, int>("Arcana",0),
				new KeyValuePair<string, int>("History",0),
				new KeyValuePair<string, int>("Investigation",2),
				new KeyValuePair<string, int>("Nature",0),
				new KeyValuePair<string, int>("Religion",0),
				new KeyValuePair<string, int>("AnimalHandling",0),
				new KeyValuePair<string, int>("Insight",1),
				new KeyValuePair<string, int>("Medicine",0),
				new KeyValuePair<string, int>("Perception",2),
				new KeyValuePair<string, int>("Survival",0),
				new KeyValuePair<string, int>("Deception",2),
				new KeyValuePair<string, int>("Intimidation",0),
				new KeyValuePair<string, int>("Performance",0),
				new KeyValuePair<string, int>("Persuasion",2),
#endif
			};
			for(int i = 0; i < kvs.Length; ++i) {
				string k = kvs[i].Key; int v = kvs[i].Value;
				bMap.Set(k, v);
				sMap.Set(k, v);
				shMap.Set(k, v);
				hTable[k] = v;
			}
			Console.WriteLine("bMap\n" + bMap.ToDebugString() + "\n");
			Console.WriteLine("sMap\n" + sMap.ToDebugString() + "\n");
			Console.WriteLine("shMap\n" + shMap.ToDebugString() + "\n");
			Console.WriteLine("htable\n" + hTable.ToDebugString() + "\n");

			for (int i = 0; i < kvs.Length; ++i) { Console.Write(kvs[i].Key + ": " + bMap.Get(kvs[i].Key) + " "); } Console.WriteLine();
			for (int i = 0; i < kvs.Length; ++i) { Console.Write(kvs[i].Key + ": " + sMap.Get(kvs[i].Key) + " "); } Console.WriteLine();
			for (int i = 0; i < kvs.Length; ++i) { Console.Write(kvs[i].Key + ": " + shMap.Get(kvs[i].Key) + " "); } Console.WriteLine();
			for (int i = 0; i < kvs.Length; ++i) { Console.Write(kvs[i].Key + ": " + hTable[kvs[i].Key] + " "); } Console.WriteLine();
			Console.WriteLine("done!");
		}
	}
}
