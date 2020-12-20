using System;
using System.Collections.Generic;

namespace CmdLineMaze {
	public class KBind {
		public ConsoleKey key;
		public Action action;
		public string description;
		public KBind(ConsoleKey key, Action action, string description = "") {
			this.key = key; this.action = action; this.description = description;
		}
	}
	public class InputListing {
		public Dictionary<ConsoleKey, List<KBind>> keyBinds = new Dictionary<ConsoleKey, List<KBind>>();
		public InputListing(params KBind[] listing) {
			for(int i = 0; i < listing.Length; ++i) {
				Add(listing[i]);
			}
		}
		public void Add(KBind kBind) {
			List<KBind> bindings;
			if (!keyBinds.TryGetValue(kBind.key, out bindings)) {
				bindings = new List<KBind>();
				keyBinds[kBind.key] = bindings;
			}
			if (bindings.IndexOf(kBind) < 0) {
				bindings.Add(kBind);
			}
		}
		public void Remove(KBind kBind) {
			List<KBind> bindings;
			if (!keyBinds.TryGetValue(kBind.key, out bindings)) {
				return;
			}
			int index = bindings.IndexOf(kBind);
			if (index >= 0) {
				bindings.RemoveAt(index);
			}
		}
		public void Add(InputListing input) {
			Dictionary<ConsoleKey, List<KBind>> kBinds = input.keyBinds;
			foreach (var kvp in kBinds) {
				for (int i = 0; i < kvp.Value.Count; ++i) {
					Add(kvp.Value[i]);
				}
			}
		}
		public void Remove(InputListing input) {
			Dictionary<ConsoleKey, List<KBind>> kBinds = input.keyBinds;
			foreach (var kvp in input.keyBinds) {
				for (int i = 0; i < kvp.Value.Count; ++i) {
					Remove(kvp.Value[i]);
				}
			}
		}
		public IList<KBind> Get(ConsoleKey key) {
			List<KBind> bindings;
			if (!keyBinds.TryGetValue(key, out bindings)) {
				return null;
			}
			return bindings;
		}
	}
	public class AppInput {
		public InputListing currentKeyBinds = new InputListing();
		public bool DoKeyPress(ConsoleKey key) {
			IList<KBind> keyBindings = currentKeyBinds.Get(key);
			if(keyBindings != null) {
				for(int i = 0; i < keyBindings.Count; ++i) {
					keyBindings[i].action.Invoke();
				}
				return true;
			}
			return false;
		}
		public void Add(InputListing input) { currentKeyBinds.Add(input); }
		public void Remove(InputListing input) { currentKeyBinds.Remove(input); }
	}
}
