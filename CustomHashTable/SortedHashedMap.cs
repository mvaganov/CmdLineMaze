using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHashTable {
	public class SortedHashedMap<KEY,VAL> where KEY : IComparable<KEY> {
		public List<KV> list;
		public Func<KEY, int> hashFunction;
		public struct KV {
			public int hash; public KEY key; public VAL val;
			public KV(int h, KEY k, VAL v) { hash = h; key = k; val = v; }
			public KV(int h, KEY k) : this(h, k, default(VAL)) { }
			public override string ToString() { return key + "(" + hash + "):" + val; }
			public class Comparer : Comparer<KV> {
				public override int Compare(KV x, KV y) { return x.hash.CompareTo(y.hash); }
			}
			public static Comparer comparer = new Comparer();
		}
		public SortedHashedMap() { }
		public SortedHashedMap(Func<KEY, int> hashFunc) { hashFunction = hashFunc; }
		public int Hash(KEY key) => Math.Abs(hashFunction != null ? hashFunction(key) : key.GetHashCode());
		KV Kv(KEY k) => new KV(Hash(k), k);
		KV Kv(KEY k, VAL v) => new KV(Hash(k), k, v);
		public Func<KEY, int> HashFunction {
			get => hashFunction;
			set => SetHashFunction(value);
		}
		public void SetHashFunction(Func<KEY, int> hFunc) {
			hashFunction = hFunc;
			List<KV> oldList = list;
			list = new List<KV>(oldList.Count);
			oldList?.ForEach(kvp => Set(kvp.key, kvp.val));
		}

		public void Set(KEY key, VAL val) => Set(Kv(key, val));
		public void Set(KV kv) {
			if(list == null) { list = new List<KV>(); }
			int bestIndex = list.BinarySearch(kv, KV.comparer);
			if(bestIndex < 0) {
				list.Insert(~bestIndex, kv);
			} else {
				bestIndex = FindExactIndex(kv, bestIndex, list);
				if (bestIndex >= 0) {
					list[bestIndex] = kv;
				} else {
					list.Insert(~bestIndex, kv);
				}
			}
		}

		int FindExactIndex(KV kvp, int index, List<KV> list) {
			while(index > 0 && list[index-1].hash == kvp.hash) { --index; }
			do {
				int compareValue = list[index].key.CompareTo(kvp.key);
				if (compareValue == 0) return index;
				if (compareValue > 0) return ~index;
				++index;
			} while (index < list.Count && list[index].hash == kvp.hash);
			return ~index;
		}

		public bool TryGet(KEY key, out KV kv) {
			kv = Kv(key);
			if (list == null) { return false; }
			int bestIndex = list.BinarySearch(kv, KV.comparer);
			if (bestIndex < 0) { return false; }
			bestIndex = FindExactIndex(kv, bestIndex, list);
			if(bestIndex < 0) { return false; }
			kv = list[bestIndex];
			return true;
		}

		public VAL Get(KEY key) {
			KV kvPair;
			if(TryGet(key, out kvPair)) { return kvPair.val; }
			throw new Exception($"map does not contain key '{key}'");
		}

		public string ToDebugString() {
			if (list == null) return "";
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < list.Count; ++i) {
				if (i > 0) sb.Append('\n');
				sb.Append(list[i]);
			}
			return sb.ToString();
		}
	}
}
