using System;
using Logger = SoulboundBackend.Common.Logging.Logger;
using UnityEngine;
using SoulboundBackend.Client.UI.Tooltip;
using System.Resources;
using SoulboundBackend.Core.Resource;
using ResourceManager = SoulboundBackend.Core.Resource.ResourceManager;
using SoulboundBackend.Client.UI.Storage;
using SoulboundBackend.Core;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract partial class Item : IStaticResettable {
		private static readonly Logger logger = Logger.CreateInstance();
		public const int DEFAULT_MAX_STACK = 256;

		public abstract string name { get; }
		public abstract ItemAspect aspect { get; }
		public abstract int maxStackSize { get; }
		public bool IsStackable => maxStackSize > 1;
		protected abstract Func<Item, TooltipData?> tooltipSupplier { get; }
		protected abstract TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }

		public virtual SlotHook? GetSlotHook() => null;

		public static GameObject InstantiateDefaultWorldPrefab() {
			return GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("droppedItem"))!;
		}

		public Tooltip? RenderTooltip(Vector2 position, Transform parent) {
			TooltipData tooltipData = tooltipSupplier?.Invoke(this) ?? Tooltip.Plain(this.name);
			TooltipRenderer renderer = new(nodeStyleProvider ?? TooltipNodeStylePresets.PresetProvider());
			Tooltip tooltip = new Tooltip(renderer, tooltipData);
			tooltip.Show(position, parent);
			return tooltip;
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
			return HashCode.Combine(name, aspect, maxStackSize, IsStackable, tooltipSupplier, nodeStyleProvider, hashedID);
		}

		public virtual void StaticReset() {
		}
	}

	public record SlotHook(Action<ItemDisplay, IItemSlot>? onAttached, Action<ItemDisplay, IItemSlot>? onDetached);
}
