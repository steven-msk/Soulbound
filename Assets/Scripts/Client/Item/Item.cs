using System;
using Logger = SoulboundBackend.Common.Logging.Logger;
using UnityEngine;
using SoulboundBackend.Client.UI.Tooltip;
using System.Resources;
using SoulboundBackend.Core.Resource;
using ResourceManager = SoulboundBackend.Core.Resource.ResourceManager;
using SoulboundBackend.Client.UI.Storage;

#nullable enable

namespace SoulboundBackend.Client.ItemSystem {
	public abstract partial class Item {
		private static readonly Logger logger = Logger.CreateInstance();
		public const int DEFAULT_MAX_STACK = 256;

		// FIXME: mismatched README version - item dev notes are obsolete

		public abstract string name { get; }
		public abstract ItemAspect aspect { get; }
		public abstract int maxStackSize { get; }
		public bool IsStackable => maxStackSize > 1;
		protected abstract Func<Item, TooltipData?> tooltipSupplier { get; }
		protected abstract TooltipRenderer.NodeStyleProvider? nodeStyleProvider { get; }
		private SlotHook? slotHook;

		public SlotHook? GetSlotHook() => slotHook;
		public void SetSlotHook(SlotHook? slotHook) => this.slotHook = slotHook;

		public static GameObject InstantiateDefaultWorldPrefab() {
			return GameObject.Instantiate(ResourceManager.Get<GameObject, ResourceGroups.Prefabs>("droppedItem"))!;
		}

		public Tooltip? RenderTooltip(Vector2 position, Transform parent) {
			try {
				TooltipData tooltipData = tooltipSupplier.Invoke(this) ?? Tooltip.Plain(this.name);
				TooltipRenderer renderer = new(nodeStyleProvider ?? TooltipNodeStylePresets.PresetProvider());
				Tooltip tooltip = new Tooltip(renderer, tooltipData);
				tooltip.Show(position, parent);
				return tooltip;
			} catch (Exception e) {
				logger.ThrowException(null, e, "Unexpected exception thrown while rendering tooltip");
				return null;
			}
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
			return HashCode.Combine(name, aspect, maxStackSize, IsStackable, tooltipSupplier, nodeStyleProvider, id);
		}
	}

	public record SlotHook(Action<ItemDisplay, IItemSlot>? onAttached, Action<ItemDisplay, IItemSlot>? onDetached);
}
