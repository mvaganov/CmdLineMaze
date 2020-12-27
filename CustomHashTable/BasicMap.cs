using System;
using System.Collections.Generic;
using System.Text;

namespace CustomHashTable {
	public class BasicMap<KEY,VAL> {
		public List<KV> list;

		public struct KV {
			public KEY key; public VAL val;
			public KV(KEY k, VAL v) { key = k; val = v; }
			public KV(KEY k) : this(k, default(VAL)) { }
			public override string ToString() { return key + ":" + val; }
		}

		public void Set(KEY key, VAL value) {
			if(list == null) { list = new List<KV>(); }
			for (int i = 0; i < list.Count; ++i) {
				if(list[i].key.Equals(key)) {
					list[i] = new KV(key, value);
					return;
				}
			}
			list.Add(new KV(key, value));
		}

		public bool TryGet(KEY key, out KV entry) {
			entry = new KV(key);
			if (list == null) return false;
			for(int i = 0; i < list.Count; ++i) {
				if(list[i].key.Equals(key)) {
					entry = list[i];
					return true;
				}
			}
			return false;
		}

		public VAL Get(KEY key) {
			KV kvPair;
			if(TryGet(key, out kvPair)) {
				return kvPair.val;
			}
			throw new Exception($"map does not contain key '{key}'");
		}

		public string ToDebugString() {
			if (list == null) return "";
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < list.Count; ++i) {
				if (i > 0) sb.Append('\n');
				sb.Append(list[i]);
			}
			return sb.ToString();
		}
	}
}
