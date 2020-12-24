namespace MazeGeneration {
	class Program {
		static void Main(string[] args) {
			int width = 43, height = 21, px = 1, py = 1, stepx = 2, stepy = 1, wallx = 1, wally = 1, erode = 4,
				seed = System.Environment.TickCount;
			string filename = "maze.txt";
			bool useSimple = true;
			if (useSimple) {
				MazeGen.show = true;
				MazeGen.WriteMaze(width, height, px, py, seed, filename);
				if (MazeGen.show) { while (System.Console.ReadKey().Key != System.ConsoleKey.Enter) ; }
			} else {
				MazeGenerator.ShowOption opt = MazeGenerator.ShowOption.EachStepPause;
				MazeGenerator.WriteMaze(width, height, px, py, stepx, stepy, wallx, wally, seed, filename, erode, opt);
				if ((opt & MazeGenerator.ShowOption.PauseOnShow) != 0) {
					while (System.Console.ReadKey().Key != System.ConsoleKey.Enter) ;
				}
			}
		}
	}
}
