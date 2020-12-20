using System;
using System.Collections.Generic;

namespace TimeSensitive {
	public class Timer {
		public static long NowRealTime => System.Environment.TickCount64;
		private long _lastUpdateRealtime = NowRealTime;
		private int _deltaTime;

		private static Timer _instance;
		public static Timer Instance => _instance != null ? _instance : _instance = new Timer();
		public static int deltaTime => Instance._deltaTime;
		public static long lastUpdateTime => Instance._lastUpdateRealtime;
		public List<TimerTask> tasks = new List<TimerTask>();
		public class TimerTask {
			public long when;
			public Action what;
			public class Comparer : IComparer<TimerTask> {
				// reverse order, so soonest objects are at the end, and easier to remove
				public int Compare(TimerTask x, TimerTask y) => -x.when.CompareTo(y.when);
			}
			public static Comparer comparer = new Comparer();
		}

		private Timer() {
			_lastUpdateRealtime = NowRealTime;
		}
		public static void setTimeout(Action action, int delayMs) => Instance.AddTask(delayMs, action);

		public void AddTask(long delayMs, Action action) {
			AddTask(new TimerTask { what = action, when = NowRealTime + delayMs });
		}

		public void AddTask(TimerTask task) {
			int properIndex = tasks.BinarySearch(task, TimerTask.comparer);
			if(properIndex < 0) {
				properIndex = ~properIndex;
			}
			tasks.Insert(properIndex, task);
		}

		public void Update() {
			int index;
			while(tasks.Count > 0 && NowRealTime > tasks[(index = tasks.Count - 1)].when) {
				tasks[index].what.Invoke();
				tasks.RemoveAt(index);
			}
			long now = NowRealTime;
			_deltaTime = (int)(now - _lastUpdateRealtime);
			_lastUpdateRealtime = now;
		}
	}
}
