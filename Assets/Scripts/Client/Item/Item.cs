using System;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract partial class Item {
		public const int DEFAULT_FULL_STACK = 256;

		private readonly string id;
		public abstract string name { get; }
		public abstract ItemAspect aspect { get; }
		public virtual int fullStackSize { get; } = DEFAULT_FULL_STACK;

		protected Item(string id) {
			this.id = id;
			ItemRegistry.Register(this);
		}

		public virtual ItemStack CreateStack(int quantity = 1) {
			return new ItemStack(this, Mathf.Clamp(quantity, 0, fullStackSize));
		}

		public override string ToString() {
			return $"item:{name}";
		}

		public override int GetHashCode() {
			return HashCode.Combine(name, aspect, fullStackSize, hashedID);
		}

		public bool IsStackable() => fullStackSize > 1;

		public string GetID() => id;
	}
}
