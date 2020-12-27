using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace CustomHashTable {
	public class HTable<KEY, VAL> : IDictionary<KEY, VAL> {
		public Func<KEY, int> hFunc = null;
		public List<List<KV>> buckets;
		public const int defaultBuckets = 8;
		int Hash(KEY key) => Math.Abs(hFunc != null ? hFunc(key) : key.GetHashCode());
		public struct KV {
			public int hash; public KEY key; public VAL val;
			public KV(int hash, KEY k) : this(hash, k, default) { }
			public KV(int h, KEY k, VAL v) { key = k; val = v; hash = h; }
			public override string ToString() => key + "(" + hash + "):" + val;
			public class Comparer : IComparer<KV> {
				public int Compare(KV x, KV y) { return x.hash.CompareTo(y.hash); }
			}
			public static Comparer comparer = new Comparer();
			public static implicit operator KeyValuePair<KEY,VAL>(KV k) => new KeyValuePair<KEY, VAL>(k.key, k.val);
		}
		private KV kv(KEY key) => new KV(Hash(key), key);
		private KV kv(KEY key, VAL val) => new KV(Hash(key), key, val);
		public HTable(Func<KEY, int> hashFunc, int bCount = defaultBuckets) { hFunc = hashFunc; BucketCount = bCount; }
		public HTable() { }
		public HTable(int bucketCount) { BucketCount = bucketCount; }
		public int Count {
			get {
				int sum = 0;
				if (buckets != null) {
					buckets.ForEach(bucket => sum += bucket != null ? bucket.Count : 0);
				}
				return sum;
			}
		}
		public int BucketCount {
			get => buckets != null ? buckets.Count : 0;
			set {
				List<List<KV>> oldbuckets = buckets;
				buckets = new List<List<KV>>(value);
				for (int i = 0; i < value; ++i) { buckets.Add(null); }
				if (oldbuckets != null) {
					oldbuckets.ForEach(b => b.ForEach(kvp => Set(kvp)));
				}
			}
		}

		public void FindEntry(KV kvp, out List<KV> bucket, out int bestIndexInBucket, out bool alreadyInHere) {
			int whichBucket = kvp.hash % buckets.Count;
			bucket = buckets[whichBucket];
			if (bucket == null) { buckets[whichBucket] = bucket = new List<KV>(); }
			bestIndexInBucket = bucket.BinarySearch(kvp, KV.comparer);
			if (bestIndexInBucket < 0) { bestIndexInBucket = ~bestIndexInBucket; }
			int i = bestIndexInBucket;
			do {
				alreadyInHere = bucket.Count > bestIndexInBucket && bucket[i].key.Equals(kvp.key);
				if (alreadyInHere) {
					bestIndexInBucket = i;
					return;
				}
			} while (i+1 < bucket.Count && bucket[i++].key.GetHashCode() == kvp.hash);
		}
		public bool Set(KV kvp) {
			if (buckets == null) { BucketCount = defaultBuckets; }
			FindEntry(kvp, out List<KV> bucket, out int bestIndexInBucket, out bool alreadyInHere);
			if (alreadyInHere) {
				bucket[bestIndexInBucket] = kvp;
			} else {
				bucket.Insert(bestIndexInBucket, kvp);
			}
			return !alreadyInHere;
		}

		public bool TryGet(KEY key, out KV entry) {
			entry = kv(key);
			if (buckets == null) return false;
			FindEntry(entry, out List<KV> bucket, out int bestIndexInBucket, out bool alreadyInHere);
			if (alreadyInHere) {
				entry = bucket[bestIndexInBucket];
				return true;
			}
			return false;
		}
		public string ToDebugString() {
			StringBuilder sb = new StringBuilder();
			for (int b = 0; b < buckets.Count; ++b) {
				if(b>0)sb.Append("\n");
				sb.Append(b.ToString()).Append(": ");
				if (buckets[b] != null) {
					for (int i = 0; i < buckets[b].Count; ++i) {
						if (i > 0) sb.Append(", ");
						sb.Append(buckets[b][i].ToString());
					}
				}
			}
			return sb.ToString();
		}
/////////////////////////////////////////////// implementing IDictionary below ////////////////////////////////////////
		public ICollection<KEY> Keys {
			get {
				List<KEY> list = new List<KEY>(Count);
				buckets?.ForEach(b => b?.ForEach(kvp => list.Add(kvp.key)));
				return list;
			}
		}

		public ICollection<VAL> Values {
			get {
				List<VAL> list = new List<VAL>(Count);
				buckets?.ForEach(b => b?.ForEach(kvp => list.Add(kvp.val)));
				return list;
			}
		}

		public bool IsReadOnly => false;

		public VAL this[KEY key] { get {
				if (TryGet(key, out KV found)) { return found.val; }
				throw new Exception("Could not find " + key + ", use TryGet instead?");
			}
			set => Add(key, value);
		}

		public void Add(KEY key, VAL value) { Set(kv(key, value)); }

		public bool ContainsKey(KEY key) {
			FindEntry(kv(key), out List<KV> bucket, out int bestIndexInBucket, out bool alreadyInHere);
			return alreadyInHere;
		}

		public bool Remove(KEY key) {
			FindEntry(kv(key), out List<KV> bucket, out int bestIndexInBucket, out bool alreadyInHere);
			if (alreadyInHere) {
				bucket.RemoveAt(bestIndexInBucket);
				return true;
			}
			return false;
		}

		public bool TryGetValue(KEY key, [MaybeNullWhen(false)] out VAL value) {
			if (TryGet(key, out KV found)) { value = found.val; return true; }
			value = default;
			return false;
		}

		public void Add(KeyValuePair<KEY, VAL> item) => Set(kv(item.Key, item.Value));

		public void Clear() {
			if (buckets == null) return;
			for(int i = 0; i < buckets.Count; ++i) { buckets[i]?.Clear(); }
		}

		public bool Contains(KeyValuePair<KEY, VAL> item) {
			FindEntry(kv(item.Key), out List<KV> bucket, out int bestIndex, out bool alreadyInHere);
			return alreadyInHere && bucket[bestIndex].val.Equals(item.Value);
		}

		public void CopyTo(KeyValuePair<KEY, VAL>[] array, int arrayIndex) {
			int index = arrayIndex;
			for(int b = 0; b < buckets.Count; ++b) {
				List<KV> bucket = buckets[b];
				if(bucket != null) {
					for(int i = 0; i < bucket.Count; ++i) {
						array[index++] = bucket[i];
					}
				}
			}
		}

		public bool Remove(KeyValuePair<KEY, VAL> item) {
			FindEntry(kv(item.Key), out List<KV> bucket, out int bestIndex, out bool alreadyInHere);
			if (alreadyInHere && item.Value.Equals(bucket[bestIndex].val)) {
				bucket.RemoveAt(bestIndex);
				return true;
			}
			return false;
		}

		public IEnumerator<KeyValuePair<KEY, VAL>> GetEnumerator() => new Enumerator(this);

		IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

		public class Enumerator : IEnumerator<KeyValuePair<KEY, VAL>> {
			HTable<KEY,VAL> htable;
			int bucket, index = -1;
			public Enumerator(HTable<KEY,VAL> htable) { this.htable = htable; }
			public KeyValuePair<KEY, VAL> Current => htable.buckets[bucket][index];
			object IEnumerator.Current => Current;
			public void Dispose() { htable = null; }
			public bool MoveNext() {
				if (htable.buckets == null) return false;
				do {
					if(htable.buckets[bucket] == null || ++index >= htable.buckets[bucket].Count) {
						++bucket;
						index = -1;
					} else {
						return true;
					}
				} while (bucket < htable.buckets.Count);
				return false;
			}
			public void Reset() { bucket = index = 0; }
		}
	}

	public static class HashFunctions {
		public static int GetDeterministicHashCode(this string str) {
			unchecked {
				int hash1 = (5381 << 16) + 5381;
				int hash2 = hash1;
				for (int i = 0; i < str.Length; i += 2) {
					hash1 = ((hash1 << 5) + hash1) ^ str[i];
					if (i == str.Length - 1)
						break;
					hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
				}
				return hash1 + (hash2 * 1566083941);
			}
		}
	}
}
