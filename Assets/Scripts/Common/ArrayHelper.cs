namespace SoulboundBackend.Common.Collections {
	public static class ArrayHelper {
		public static T[,] CompressTo2D<T>(this T[] input, int rows, int columns) {
			UnityEngine.Debug.Assert(input.Length == rows * columns);
			T[,] result = new T[rows, columns];

			for (int i = 0; i < input.Length; i++) {
				int row = i / columns;
				int column = i % columns;
				result[row, column] = input[i];
			}
			return result;
		}

		public static T[] Flatten<T>(this T[,] other) {
			int rows = other.GetLength(0);
			int columns = other.GetLength(1);
			T[] result = new T[rows * columns];

			int index = 0;
			for (int r = 0; r < rows; r++) {
				for (int c = 0; c < columns; c++) {
					result[index++] = other[r, c];
				}
			}
			return result;
		}
	}
}
