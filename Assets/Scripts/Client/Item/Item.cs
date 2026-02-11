using System;
using Logger = SoulboundBackend.Common.Logging.Logger;
using UnityEngine;
using SoulboundBackend.Client.UI.Tooltip;
using System.Resources;
using SoulboundBackend.Core.Resource;
using ResourceManager = SoulboundBackend.Core.Resource.AssetManager;
using SoulboundBackend.Core;
using SoulboundBackend.Client.UI.Storage;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract partial class Item {
		private static readonly Logger logger = Logger.CreateInstance();
		public const int DEFAULT_MAX_STACK = 256;

		public abstract string name { get; }
		public abstract ItemAspect aspect { get; }
		public virtual int maxStackSize { get; } = DEFAULT_MAX_STACK;
		public bool IsStackable => maxStackSize > 1;

		public virtual void OnAttachedInSlot(IItemSlot slot) {
		}

		public virtual void OnDetachedFromSlot(IItemSlot slot) {
		}

		public static int CustomMaxStack(int maxStack) => maxStack;

		public bool HasCapability<T>() where T : IItemCapability {
			return typeof(T).IsAssignableFrom(this.GetType());
		}

		public bool HasCapability(Type type) {
			return type.IsAssignableFrom(this.GetType());
		}

		public override string ToString() {
			return name;
		}

		public override int GetHashCode() {
			return HashCode.Combine(name, aspect, maxStackSize, IsStackable, hashedID);
		}
	}
}
