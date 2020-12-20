using System;
using System.Collections.Generic;
using System.Text;

namespace CmdLineMaze {
	class Program {
		public static void Main(string[] args) {
			Game g = new Game();
			g.Init();
			while(g.status == Game.GameStatus.Running) {
				g.Draw();
				g.Input();
				g.Update();
			}
			g.Release();
		}
	}
}
