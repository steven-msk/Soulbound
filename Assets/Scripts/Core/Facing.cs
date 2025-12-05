using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core {
	public readonly struct Facing {
		public readonly Vector2 direction;
		public float x => direction.x;
		public float y => direction.y;
		public float angle => Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

		public Facing(float x) {
			direction = new Vector2(x, 0f).normalized;
		}

		public Facing(Vector2 direction) {
			this.direction = direction.normalized;
		}

		public Facing(float x, float y) {
			this.direction = new Vector2(x, y).normalized;
		}

		public static Facing FromAngle(float angleDeg) {
			float rad = angleDeg * Mathf.Deg2Rad;
			return new Facing(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
		}

		public static Facing FromScale(float signX) {
			return new Facing(signX < 0 ? -1f : 0f, 0f);
		}

		public static Facing Left => new Facing(-1f);
		public static Facing Right => new Facing(1f);
		public static Facing Up => new Facing(0f, 1f);
		public static Facing Down => new Facing(0f, -1f);

		public bool IsHorizontal => Mathf.Abs(direction.y) < 0.0001f;
	}
}
