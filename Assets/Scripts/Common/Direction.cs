using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundBackend.Common.Math {
	public enum Direction {
		Up,
		Down,
		Left,
		Right
	}

	public static class DirectionUtility {
		public static Direction Opposite(this Direction direction) {
			return direction switch {
				Direction.Up => Direction.Down,
				Direction.Down => Direction.Up,
				Direction.Left => Direction.Right,
				Direction.Right => Direction.Left,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public static Direction RotateClockwise(this Direction direction) {
			return direction switch {
				Direction.Up => Direction.Right,
				Direction.Right => Direction.Down,
				Direction.Down => Direction.Left,
				Direction.Left => Direction.Up,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public static Direction RotateCounterClockwise(this Direction direction) {
			return direction switch {
				Direction.Up => Direction.Left,
				Direction.Left => Direction.Down,
				Direction.Down => Direction.Right,
				Direction.Right => Direction.Up,
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public static (int x, int y) AsVector(this Direction direction) {
			return direction switch {
				Direction.Up => (0, 1),
				Direction.Down => (0, -1),
				Direction.Left => (-1, 0),
				Direction.Right => (1, 0),
				_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
			};
		}

		public static Vector2Int AsVector2Int(this Direction direction) {
			var (x, y) = direction.AsVector();
			return new Vector2Int(x, y);
		}

		public static Direction FromVector(int x, int y) {
			if (x == 0 && y == 1) return Direction.Up;
			if (x == 0 && y == -1) return Direction.Down;
			if (x == -1 && y == 0) return Direction.Left;
			if (x == 1 && y == 0) return Direction.Right;
			throw new ArgumentException("Invalid vector for direction", nameof(x));
		}

		public static Direction FromVector((int x, int y) vector) => FromVector(vector.x, vector.y);

		public static IEnumerable<BlockPos> GetCardinalNeighbors(this BlockPos pos) {
			foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
				yield return pos + direction.AsVector();
			}
		}

		public static bool IsAdjacent(this BlockPos pos1, BlockPos pos2) {
			int dx = Mathf.Abs(pos1.x - pos2.x);
			int dy = Mathf.Abs(pos1.y - pos2.y);
			return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
		}

		public static Direction? GetDirectionTo(this BlockPos from, BlockPos to) {
			if (from.x == to.x) {
				if (from.y < to.y) return Direction.Up;
				if (from.y > to.y) return Direction.Down;
			} else if (from.y == to.y) {
				if (from.x < to.x) return Direction.Right;
				if (from.x > to.x) return Direction.Left;
			}
			return null;
		}

		public static BlockPos GetAdjacent(this BlockPos blockPos, Direction direction) => blockPos + direction.AsVector2Int();
	}
}
