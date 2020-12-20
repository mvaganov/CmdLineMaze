﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MazeGeneration {
	public class MazeGenerator {
		const char Filled = '#', Walkable = ' ';
		public enum ShowOption { None = 0, EachStep = 1, PauseOnShow = 2, EachStepPause = 3, Final = 4, FinalPause = 6 }
		ShowOption showOption = ShowOption.Final;
		public static void WriteMaze(int width, int height, int startx, int starty,
			int stepx, int stepy, int wallx, int wally, int seed, string filename, ShowOption option = ShowOption.None) {
			WriteMaze(new Coord(width, height), new Coord(startx, starty),
				new Coord(stepx, stepy), new Coord(wallx, wally), seed, filename, option);
		}
		public static void WriteMaze(Coord size, Coord start, Coord step, Coord wall, int seed, string filename,
			ShowOption option = ShowOption.None) {
			string maze = CreateMaze(size, start, step, wall, seed, option);
			System.IO.File.WriteAllText(filename, maze);
		}
		public static string CreateMaze(Coord size, Coord start, Coord step, Coord wall, int seed, ShowOption option = ShowOption.None) {
			MazeGenerator mg = new MazeGenerator(seed);
			mg.showOption = option;
			char[,] map = mg.Generate(size, start, step, wall);
			return ToString(map);
		}
		public static string ToString(char[,] map) {
			StringBuilder sb = new StringBuilder();
			Coord size = map.GetSize(), cursor;
			for (cursor.row = 0; cursor.row < size.row; ++cursor.row) {
				for (cursor.col = 0; cursor.col < size.col; ++cursor.col) {
					sb.Append(map.At(cursor));
				}
				sb.Append('\n');
			}
			return sb.ToString();
		}
		Random random;
		MazeGenerator(int seed) {
			random = new Random(seed);
		}
		char[,] Generate(Coord size, Coord start, Coord step, Coord wall) {
			char[,] map = new char[size.row, size.col];
			map.Fill(Filled);
			if (start.IsWithin(size)) {
				map.SetAt(start, step, Walkable);
			}
			MazeWalk(map, start, step, wall);
			return map;
		}
		public void Show(char[,] map) {
			Console.SetCursorPosition(0, 0);
			Console.WriteLine(ToString(map));
			if(showOption == ShowOption.EachStepPause) Console.ReadKey();
		}
		void MazeWalk(char[,] map, Coord startingPoint, Coord stepSize, Coord wallSize) {
			List<Coord> possibleIntersections = new List<Coord>();
			possibleIntersections.Add(startingPoint);
			if((showOption & ShowOption.EachStep) != 0) Show(map);
			while (possibleIntersections.Count > 0) {
				if (MazeWalkOneStep(map, possibleIntersections, stepSize, wallSize)) {
					if ((showOption & ShowOption.EachStep) != 0) Show(map);
				}
			}
			if ((showOption & ShowOption.Final) != 0) Show(map);
		}
		bool MazeWalkOneStep(char[,] map, List<Coord> possibleIntersections, Coord stepSize, Coord wallSize) {
			int intersectionToTry = random.Next(possibleIntersections.Count);
			Coord position = possibleIntersections[intersectionToTry];
			List<Coord> possibleNextSteps = PossibleNextSteps(position, stepSize, wallSize, map);
			if (possibleNextSteps.Count == 0) {
				possibleIntersections.RemoveAt(intersectionToTry);
				return false;
			}
			int whichStepToTake = random.Next(possibleNextSteps.Count);
			Coord dir = possibleNextSteps[whichStepToTake];
			position += dir * stepSize;
			map.SetAt(position, stepSize, Walkable);
			int relevantCount = dir.col != 0 ? wallSize.col : wallSize.row;
			for (int i = 0; i < relevantCount; ++i) {
				position += dir;
				map.SetAt(position, stepSize, Walkable);
			}
			possibleIntersections.Add(position);
			return true;
		}
		static readonly Coord[] _directions = new Coord[] { Coord.Up, Coord.Left, Coord.Down, Coord.Right };
		public List<Coord> PossibleNextSteps(Coord start, Coord stepSize, Coord wallSize, char[,] map) {
			List<Coord> possibleNext = new List<Coord>();
			for (int i = 0; i < _directions.Length; ++i) {
				if (IsMapWalkable(start, _directions[i], stepSize, wallSize, map) == State.Unclaimed) {
					possibleNext.Add(_directions[i]);
				}
			}
			return possibleNext;
		}
		public enum State { None, Unclaimed, Wall, Empty, Incomplete, OutOfBounds, Unknown }
		State IsMapWalkable(Coord position, Coord direction, Coord stepSize, Coord wallSize, char[,] map) {
			Coord size = map.GetSize();
			Coord next = position + direction * stepSize;
			if (!next.IsWithin(size)) return State.OutOfBounds;
			Coord end = next + direction * wallSize;
			if (!end.IsWithin(size)) return State.OutOfBounds;
			char n = map.At(next), e = map.At(end);
			switch (n) {
			case Filled:
				switch (e) {
				case Filled: return State.Unclaimed;
				case Walkable: return State.Wall;
				default: return State.Unknown;
				}
			case Walkable:
				switch (e) {
				case Filled: return State.Incomplete;
				case Walkable: return State.Empty;
				default: return State.Unknown;
				}
			}
			return State.Unknown;
		}
	}
}