using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Direction {
    Up,
    Down,
    Left,
    Right
}

public static class Directions {
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

    public static void ForEachAdjacent(this BlockPos pos, Action<Direction, BlockPos> neighborAction) {
        foreach (Direction direction in Enum.GetValues(typeof(Direction))) {
            BlockPos neighborPos = pos + direction.AsVector();
            neighborAction.Invoke(direction, neighborPos);
        }
    }
}
