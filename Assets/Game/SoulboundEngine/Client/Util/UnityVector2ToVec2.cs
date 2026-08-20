using SoulboundEngine.Common.Math;
using UnityEngine;

namespace SoulboundEngine.Client.Util {
	public static class UnityVector2ToVec2 {
		public static Vec2d ToVec2d(this Vector2 v) {
			return new Vec2d(v.x, v.y);
		}
		public static Vector2 ToVector2(this Vec2d v) {
			return new Vector2((float)v.x, (float)v.y);
		}

		public static Vec2f ToVec2f(this Vector2 v) {
			return new Vec2f(v.x, v.y);
		}
		public static Vector2 ToVector2(this Vec2f v) {
			return new Vector2(v.x, v.y);
		}

		public static Vector2 ToVector2(this Vec2i v) {
			return new Vector2(v.x, v.y);
		}
		public static Vec2i FloorToVec2i(this Vector2 v) {
			return new Vec2i(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
		}

		public static Vector2Int ToVector2Int(this Vec2i v) {
			return new Vector2Int(v.x, v.y);
		}
		public static Vec2i ToVec2i(this Vector2Int v) {
			return new Vec2i(v.x, v.y);
		}
	}
}
