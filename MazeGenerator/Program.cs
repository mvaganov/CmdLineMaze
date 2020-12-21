namespace MazeGeneration {
	class Program {
		static void Main(string[] args) {
			int width = 25, height = 15, px = 1, py = 1, stepx = 2, stepy = 1, wallx = 1, wally = 1, 
				seed = System.Environment.TickCount;
			string filename = "maze.txt";
			//MazeGenerator.ShowOption opt = MazeGenerator.ShowOption.Final;
			//MazeGenerator.WriteMaze(width, height, px, py, stepx, stepy, wallx, wally, seed, filename, opt);
			MazeGen.show = false;
			MazeGen.WriteMaze(width, height, px, py, seed, filename);
		}
	}
}
