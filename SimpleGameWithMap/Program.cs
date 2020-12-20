using System;
namespace SimpleGameWithmap {
	class Program2 {
		public static void Main(string[] args) {
			string mapStr =
			".............................." +
			".............................." +
			".............................." +
			".............................." +
			".................########....." +
			"................#............." +
			"................#..#####......" +
			"................#......###...." +
			"................#####....###.." +
			"................#..........#.." +
			"................#..........#.." +
			"................#..........#.." +
			"................#..........#.." +
			"................############.." +
			"..............................";
			char[,] map = new char[15, 30];
			int strIndex = 0;
			for (int row = 0; row < map.GetLength(0); ++row) {
				for (int col = 0; col < map.GetLength(1); ++col) {
					map[row, col] = mapStr[strIndex++];
				}
			}
			int width = 30, height = 15, x = 3, y = 4;
			char player = '@';//, world = '.';
			bool running = true;
			while (running) {
				Console.SetCursorPosition(0, 0);
				for (int row = 0; row < height; ++row) {
					for (int col = 0; col < width; ++col) {
						Console.Write(map[row, col]);
					}
					Console.Write('\n');
				}
				Console.SetCursorPosition(x, y);
				Console.Write(player);
				Console.SetCursorPosition(0, height);
				ConsoleKeyInfo userInput = Console.ReadKey();
				switch (userInput.KeyChar) {
				case 'w': --y; break;
				case 'a': --x; break;
				case 's': ++y; break;
				case 'd': ++x; break;
				case 'q': case (char)27: running = false; break;
				}
			}
		}
	}
}
