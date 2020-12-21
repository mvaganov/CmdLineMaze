using System;
using System.Collections.Generic;

namespace CmdLineMaze {
	public interface IUpdatable { void Update(); }
	public class Game {
		public enum GameStatus { None, Running, Ended }
		public GameStatus status;
		AppInput appInput;
		Map2d screen, backbuffer;
		Map2d maze;
		List<IDrawable> drawList = new List<IDrawable>();
		List<IUpdatable> updateList = new List<IUpdatable>();
		List<ConsoleKeyInfo> inputQueue = new List<ConsoleKeyInfo>();
		Coord mapOffset = Coord.Zero;
		EntityMobileObject player;

		public void Init() {
			status = GameStatus.Running;
			Coord screenSize = new Coord(60, 20);
			screen = new Map2d(screenSize, '\0');
			backbuffer = new Map2d(screenSize, '\0');
			appInput = new AppInput();
			void MapScroll(Coord dir) { mapOffset -= dir; }
			void PlayerMove(Coord dir) {
				player.Move(dir);
				Coord p = player.position + mapOffset;
				if (p.col < 2) { MapScroll(Coord.Left); }
				if (p.row < 2) { MapScroll(Coord.Up); }
				if (p.col >= screenSize.col-2) { MapScroll(Coord.Right); }
				if (p.row >= screenSize.row-2) { MapScroll(Coord.Down); }
			}
			int controlSchemeIndex = 0;
			InputListing[] controlSchema = new InputListing[] {
			new InputListing(
				new KBind(ConsoleKey.W, () => PlayerMove(Coord.Up), "move player up"),
				new KBind(ConsoleKey.A, () => PlayerMove(Coord.Left), "move player left"),
				new KBind(ConsoleKey.S, () => PlayerMove(Coord.Down), "move player down"),
				new KBind(ConsoleKey.D, () => PlayerMove(Coord.Right), "move player right"),
				new KBind(ConsoleKey.UpArrow, () => PlayerMove(Coord.Up), "move player up"),
				new KBind(ConsoleKey.LeftArrow, () => PlayerMove(Coord.Left), "move player left"),
				new KBind(ConsoleKey.DownArrow, () => PlayerMove(Coord.Down), "move player down"),
				new KBind(ConsoleKey.RightArrow, () => PlayerMove(Coord.Right), "move player right")),
			new InputListing(
				new KBind(ConsoleKey.UpArrow, () => MapScroll(Coord.Up), "pan map up"),
				new KBind(ConsoleKey.LeftArrow, () => MapScroll(Coord.Left), "pan map left"),
				new KBind(ConsoleKey.DownArrow, () => MapScroll(Coord.Down), "pan map down"),
				new KBind(ConsoleKey.RightArrow, () => MapScroll(Coord.Right), "pan map right"))
			};
			InputListing system = new InputListing(
				new KBind(ConsoleKey.F1, PrintKeyBindings, "show key bindings"),
				new KBind(ConsoleKey.M, ()=> {
					appInput.Remove(controlSchema[controlSchemeIndex]);
					++controlSchemeIndex; controlSchemeIndex %= controlSchema.Length;
					appInput.Add(controlSchema[controlSchemeIndex]);
				}, "switch input mode"));
			appInput.Add(system);
			appInput.Add(controlSchema[controlSchemeIndex]);
			string mazeFile = "maze.txt";
			MazeGeneration.MazeGen.WriteMaze(100, 51, 1, 1, 123, mazeFile);
			maze = Map2d.LoadFromFile(mazeFile);
			player = new EntityMobileObject("player", new ConsoleTile('@', ConsoleColor.Green), new Coord(1, 1));
			player.onUpdate += () => { UpdateMob(player); };
			drawList.Add(maze);
			drawList.Add(player);
			updateList.Add(player);
		}

		void UpdateMob(EntityMobileObject mob) {
			if (!mob.position.IsWithin(maze.GetSize()) || maze[mob.position].letter == '#') {
				mob.position = mob.lastValidPosition;
			} else {
				mob.lastValidPosition = mob.position;
			}
		}

		void PrintKeyBindings() {
			Coord screenSize = screen.GetSize();
			screen.Fill(new ConsoleTile('.', ConsoleColor.Black, ConsoleColor.DarkGray), new Rect(2, 2, 40, screenSize.row - 4));
			screen.Render(Coord.Zero, backbuffer);
			var binds = appInput.currentKeyBinds.keyBinds;
			Console.BackgroundColor = ConsoleColor.DarkGray;
			int kbindIndex = 0;
			foreach (var kbindEntry in binds) {
				List<KBind> kbinds = kbindEntry.Value;
				for (int i = 0; i < kbinds.Count; ++i) {
					KBind kbind = kbinds[i];
					Console.SetCursorPosition(3, 3 + kbindIndex);
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write(kbind.key);
					Console.ForegroundColor = ConsoleColor.Gray;
					Console.Write(" ");
					Console.Write(kbind.description);
					++kbindIndex;
					if (kbindIndex >= screenSize.row - 5) break;
				}
				if (kbindIndex >= screenSize.row - 5) break;
			}
			ConsoleTile.DefaultTile.ApplyColor();
			Console.ReadKey();
			screen.Fill(ConsoleTile.DefaultTile);
			backbuffer.Fill(ConsoleTile.DefaultTile);
		}

		public void Release() {
		}

		public void Draw() {
			screen.Fill(ConsoleTile.DefaultTile);
			ConsoleTile[,] drawBuffer = screen.GetRawMap();
			for (int i = 0; i < drawList.Count; ++i) {
				drawList[i].Draw(drawBuffer, mapOffset);
			}
			screen.Render(Coord.Zero, backbuffer);
			Map2d temp = screen; screen = backbuffer; backbuffer = temp;
			ConsoleTile.DefaultTile.ApplyColor();
			Console.SetCursorPosition(0, screen.Height);
		}

		public void Update() {
			do {
				if (inputQueue.Count > 0) {
					ConsoleKeyInfo input = inputQueue[0];
					inputQueue.RemoveAt(0);
					if (!appInput.DoKeyPress(input)) {
						Console.WriteLine(input.Key);
					}
				}
				for (int i = 0; i < updateList.Count; ++i) {
					updateList[i].Update();
				}
			} while (inputQueue.Count > 0);
		}

		public void Input() {
			ConsoleKeyInfo input;
			while (Console.KeyAvailable) {
				input = Console.ReadKey();
				inputQueue.Add(input);
				if (input.Key == ConsoleKey.Escape) { status = GameStatus.Ended; return; }
			}
		}
	}
}
