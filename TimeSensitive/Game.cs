using System;

namespace TimeSensitive {
	public class Game {
		public enum GameStatus { None, Running, Ended }
		public GameStatus status;
		public ConsoleKeyInfo input;
		AnimatingChar spinnyPipe;
		public bool dirty = false;
		public void Init() {
			status = GameStatus.Running;
			spinnyPipe = new AnimatingChar("|/-\\|/-\\##", 100);
		}
		public void Release() {
		}
		public void Draw() {
			if (!dirty) return;
			Console.SetCursorPosition(0, 0);
			Console.WriteLine((char)spinnyPipe);
			dirty = false;
		}
		public void Update(int deltaTime) {
			dirty |= spinnyPipe.Update(deltaTime);
		}
		public void Input() {
			if (Console.KeyAvailable) { input = Console.ReadKey(); }
		}
	}
}
