using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

	public static T[] Flatten<T>(this T[,] source) {
		int rows = source.GetLength(0);
		int columns = source.GetLength(1);
		T[] result = new T[rows * columns];

		int index = 0;
		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < columns; c++) {
				result[index++] = source[r, c];
			}
		}
		return result;
	}
}
