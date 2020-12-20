namespace MazeGeneration {
	public struct Coord {
		public short row, col; // why 2 shorts?
		public Coord(int col, int row) { // why int?
			this.col = (short)col; this.row = (short)row;
		}
		public short X => col; // what is going on here?
		public short Y => row;
		public static Coord Zero = new Coord(0, 0);
		public static Coord Up = new Coord(0, -1);
		public static Coord Left = new Coord(-1, 0);
		public static Coord Down = new Coord(0, 1);
		public static Coord Right = new Coord(1, 0);
		public static Coord operator +(Coord a, Coord b) {
			return new Coord(a.col + b.col, a.row + b.row);
		}
		public static Coord operator *(Coord a, Coord b) {
			return new Coord(a.col * b.col, a.row * b.row);
		}
		public bool IsWithin(Coord boundary) {
			return col >= 0 && row >= 0 && col < boundary.col && row < boundary.row;
		}
	}
	public static class MatrixCoordExtension { // what is going on here?
		public static Coord GetSize<TYPE>(this TYPE[,] matrix) {
			return new Coord(matrix.GetLength(1), matrix.GetLength(0));
		}
		public static TYPE At<TYPE>(this TYPE[,] matrix, Coord coord) {
			return matrix[coord.row, coord.col];
		}
		public static void SetAt<TYPE>(this TYPE[,] matrix, Coord position, TYPE value) {
			matrix[position.row, position.col] = value;
		}
		public static void SetAt<TYPE>(this TYPE[,] matrix, Coord position, Coord size, TYPE value) {
			Coord cursor;
			for (cursor.row = 0; cursor.row < size.row; ++cursor.row) {
				for (cursor.col = 0; cursor.col < size.col; ++cursor.col) {
					matrix.SetAt(cursor+position, value); ;
				}
			}
		}
		public static void Fill<TYPE>(this TYPE[,] matrix, TYPE value) {
			SetAt(matrix, Coord.Zero, matrix.GetSize(), value);
		}
	}

}
