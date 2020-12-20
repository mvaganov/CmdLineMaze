using System;
using System.Collections.Generic;

namespace CmdLineMaze {
	public class Game {
		public enum GameStatus { None, Running, Ended }
		public GameStatus status;
		public ConsoleKeyInfo input;
		public AppInput appInput;
		public InputListing playerControl, system, mapScroll;
		public Map2d screen, backbuffer;
		public List<Drawable> drawList = new List<Drawable>();
		public List<ConsoleKeyInfo> inputQueue = new List<ConsoleKeyInfo>();
		public Coord mapOffset = Coord.Zero;

		public void Init() {
			status = GameStatus.Running;
			Coord screenSize = new Coord(60, 20);
			screen = new Map2d(screenSize, '\0');
			backbuffer = new Map2d(screenSize, '\0');
			appInput = new AppInput();
			playerControl = new InputListing(
				new KBind(ConsoleKey.UpArrow, () => PlayerMove(Coord.Up), "move player up"),
				new KBind(ConsoleKey.LeftArrow, () => PlayerMove(Coord.Left), "move player left"),
				new KBind(ConsoleKey.DownArrow, () => PlayerMove(Coord.Down), "move player down"),
				new KBind(ConsoleKey.RightArrow, () => PlayerMove(Coord.Right), "move player right"));
			system = new InputListing(
				new KBind(ConsoleKey.Oem2, ShowInputHelp, "show this key bind listing"));
			mapScroll = new InputListing(
				new KBind(ConsoleKey.UpArrow, () => MapScroll(Coord.Up), "move player up"),
				new KBind(ConsoleKey.LeftArrow, () => MapScroll(Coord.Left), "move player left"),
				new KBind(ConsoleKey.DownArrow, () => MapScroll(Coord.Down), "move player down"),
				new KBind(ConsoleKey.RightArrow, () => MapScroll(Coord.Right), "move player right"));
			appInput.Add(playerControl);
			appInput.Add(system);
			appInput.Add(mapScroll);
			MazeGeneration.MazeGenerator.WriteMaze(100, 51, 1, 1, 2, 1, 1, 1, 123, "maze.txt");
			Map2d maze = Map2d.LoadFromFile("maze.txt");
			drawList.Add(maze);
		}

		public void Release() {
		}

		public void Draw() {
			screen.Fill(ConsoleTile.DefaultTile);
			ConsoleTile[,] drawBuffer = screen.GetRawMap();
			for (int i = 0; i < drawList.Count; ++i) {
				drawList[i].Draw(drawBuffer, mapOffset); // TODO how is this memory leaking? is it a closure?
			}
			screen.Render(Coord.Zero, backbuffer);
			Map2d temp = screen; screen = backbuffer; backbuffer = temp;
			ConsoleTile.DefaultTile.ApplyColor();
			Console.SetCursorPosition(0, screen.Height);
			Console.WriteLine(-mapOffset);
		}

		public void Update() {
		}

		public bool Input() {
			if (!Console.KeyAvailable) { input = new ConsoleKeyInfo(); return false; }
			do {
				input = Console.ReadKey();
				inputQueue.Add(input);
				if (input.Key == ConsoleKey.Escape) { status = GameStatus.Ended; return true; }
			} while (Console.KeyAvailable);
			while (inputQueue.Count > 0) {
				input = inputQueue[0];
				inputQueue.RemoveAt(0);
				if (!appInput.DoKeyPress(input.Key)) {
					Console.WriteLine(input.Key);
				}
			}
			return true;
		}

		public void PlayerMove(Coord dir) { }
		public void MapScroll(Coord dir) { mapOffset -= dir; }
		public void ShowInputHelp() { }
	}
}
