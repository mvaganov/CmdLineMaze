﻿using System;
using System.Collections.Generic;

namespace CmdLineMaze {
	public class KBind {
		public ConsoleKey key;
		public Action action;
		public string description;
		public enum Press { None, Pressed, Unpressed }
		public Press shift, ctrl, alt;
		public KBind(ConsoleKey key, Action action, string description = "", Press shift = Press.None,
			Press ctrl = Press.None, Press alt = Press.None) {
			this.key = key; this.action = action; this.description = description;
			this.shift = shift; this.ctrl = ctrl; this.alt = alt;
		}
		public bool IsSatisfied(ConsoleKeyInfo keyinfo) {
			bool s = shift== Press.None || keyinfo.Modifiers.HasFlag(ConsoleModifiers.Shift)  == (shift == Press.Pressed);
			bool c = ctrl == Press.None || keyinfo.Modifiers.HasFlag(ConsoleModifiers.Control)== (ctrl == Press.Pressed);
			bool a = alt  == Press.None || keyinfo.Modifiers.HasFlag(ConsoleModifiers.Alt)    == (alt == Press.Pressed);
			return s && c && a;
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
		public bool DoKeyPress(ConsoleKeyInfo key) {
			IList<KBind> kBinds = currentKeyBinds.Get(key.Key);
			if(kBinds != null) {
				bool active = false;
				for(int i = 0; i < kBinds.Count; ++i) {
					if (kBinds[i].IsSatisfied(key)) {
						kBinds[i].action.Invoke();
						active = true;
					}
				}
				return active;
			}
			return false;
		}
		public void Add(InputListing input) { currentKeyBinds.Add(input); }
		public void Remove(InputListing input) { currentKeyBinds.Remove(input); }
	}
}
