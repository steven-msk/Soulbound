using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ArrayHelper {
	public static T[,] CompressTo2D<T>(T[] input, int rows, int columns) {
		Debug.Assert(input.Length == rows * columns);
		T[,] result = new T[rows, columns];

		for (int i = 0; i < input.Length; i++) {
			int row = i / columns;
			int column = i % columns;
			result[row, column] = input[i];
		}
		return result;
	}

}
public static partial class List {
	public static List<T> Empty<T>() => new List<T>();
}
