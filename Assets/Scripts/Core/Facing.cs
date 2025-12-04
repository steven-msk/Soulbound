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

		public Facing(float x) {
			direction = new Vector2(x, 0f).normalized;
		}

		public Facing(Vector2 direction) {
			this.direction = direction.normalized;
		}

		public Facing(float x, float y) {
			this.direction = new Vector2(x, y).normalized;
		}

		public static Facing Left => new Facing(-1f);
		public static Facing Right => new Facing(1f);
		public static Facing Up => new Facing(0f, 1f);
		public static Facing Down => new Facing(0f, -1f);

		public bool IsHorizontal => Mathf.Abs(direction.y) < 0.0001f;
	}
}
