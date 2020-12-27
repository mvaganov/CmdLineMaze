using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHashTable {
	public class SortedMap<KEY,VAL> where KEY : IComparable<KEY> {
		public List<KV> list;

		public struct KV {
			public KEY key; public VAL val;
			public KV(KEY k, VAL v) { key = k; val = v; }
			public KV(KEY k) : this(k, default(VAL)) { }
			public override string ToString() { return key + ":" + val; }
			public class Comparer : Comparer<KV> {
				public override int Compare(KV x, KV y) { return x.key.CompareTo(y.key); }
			}
			public static Comparer comparer = new Comparer();
		}

		public void Set(KEY key, VAL value) {
			if(list == null) { list = new List<KV>(); }
			KV kv = new KV(key, value);
			int bestIndex = list.BinarySearch(kv, KV.comparer);
			if(bestIndex < 0) {
				list.Insert(~bestIndex, kv);
			} else {
				list[bestIndex] = kv;
			}
		}

		public bool TryGet(KEY key, out KV kv) {
			kv = new KV(key);
			if (list == null) { return false; }
			int bestIndex = list.BinarySearch(kv, KV.comparer);
			if (bestIndex < 0) { return false; }
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
