﻿using System;
using System.Threading;

namespace TimeSensitive {
    public class Program {
        static void Main(string[] args) {
            Timer.Instance.Init();
            Game game = new Game();
            game.Init();
            long throttle;
            Timer.Instance.AddTask(1000, () => { Console.WriteLine("three"); });
            Timer.Instance.AddTask(2000, () => { Console.WriteLine("two  "); });
            Timer.Instance.AddTask(3000, () => { Console.WriteLine("one  "); });
            Timer.Instance.AddTask(4000, () => { Console.WriteLine("GO!  "); });
            while (game.status == Game.GameStatus.Running) {
                throttle = Timer.NowRealTime + 100;
                game.Draw();
                game.Input();
                Timer.Instance.Update();
                game.Update(Timer.deltaTime);
                while (Timer.NowRealTime < throttle && !Console.KeyAvailable) { Thread.Sleep(1); }
            }
            game.Release();
            Timer.Instance.Release();
        }
    }
}
