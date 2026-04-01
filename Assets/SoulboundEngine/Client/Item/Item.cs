using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem {
	public abstract partial class Item : IIdentifierProvider {
		public const int DEFAULT_FULL_STACK = 256;

		private readonly Identifier identifier;
		public abstract string name { get; }
		public virtual int fullStackSize { get; } = DEFAULT_FULL_STACK;

		protected Item(Identifier identifier) {
			this.identifier = identifier;
		}

		public abstract ItemRenderData GetRenderData(ItemStack itemStack);

		public virtual ItemStack CreateStack(int quantity = 1) {
			return new ItemStack(this, Mathf.Clamp(quantity, 0, fullStackSize));
		}

		public override string ToString() => identifier.ToString();

		public override int GetHashCode() {
			return HashCode.Combine(identifier, fullStackSize);
		}

		public bool IsStackable() => fullStackSize > 1;

		public Identifier GetIdentifier() => identifier;
	}
}
