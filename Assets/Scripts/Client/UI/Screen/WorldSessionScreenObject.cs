using SoulboundBackend.Client.Input;
using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.Container;
using SoulboundBackend.Client.ItemSystem.Container.View;
using SoulboundBackend.Client.UI.Screens;

using SoulboundBackend.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Screen = SoulboundBackend.Client.UI.Screens.Screen;

#nullable enable

namespace SoulboundBackend.Client.UI.Screens {
	[RequireComponent(typeof(RectTransform))]
	public class WorldSessionScreenObject : ScreenObject, IItemContainerScreenScope, IInputContext {
		private RectTransform rect = null!;
		private TransitStack transitStack = null!;
		private SlotDragState? dragState;
		private readonly List<UIItemContainerNode> openContainers = new();
		private ITransitStackHandle? currentTransitStackHandle;
		private Vector2 pointerPosition;
		int IInputContext.priority => 5000;

		public new void Init(Screen screen) {
			base.Init(screen);
			this.transitStack = new TransitStack(this);
			rect = GetComponent<RectTransform>();
			Soulbound.instance.GetInputManager().PushContext(this);
		}

		public bool TryBeginDrag(ItemStack stack, SlotRef slotRef, PointerEventData.InputButton button) {
			if (InDragState() || stack == null) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { slotRef };

			dragState = new SlotDragState(slotRef.container) {
				stack = stack.Clone(),
				origin = slotRef,
				draggedSlots = draggedSlots,
				button = button,
				quantitySnapshots = CreateQuantitySnapshots(),
			};
			return true;
		}

		private Dictionary<SlotRef, int> CreateQuantitySnapshots() {
			Dictionary<SlotRef, int> snapshots = new();

			foreach (var node in openContainers) {
				Dictionary<int, int> quantities = GetQuantitySnapshotForContainer(node.container);

				foreach (var kvp in quantities) {
					SlotRef slotRef = new(node.container, kvp.Key);
					snapshots[slotRef] = kvp.Value;
				}
			}
			return snapshots;
		}

		private Dictionary<int, int> GetQuantitySnapshotForContainer(IItemContainer container) {
			return container.GetAllSlots()
					.Where(i => container.GetSlot(i).GetStack()?.quantity > 0)
					.ToDictionary(i => i, i => container.GetSlot(i).GetStack()!.quantity);
		}

		public void EndDrag() => dragState = null;

		public void ExtendDrag(SlotRef slotRef) {
			dragState?.ExtendDrag(slotRef);
		}

		public bool InDragState() => dragState != null;

		public SlotDragState? GetDragState() => dragState;

		public IEnumerable<IItemContainer> GetOpenContainers() {
			foreach (var node in openContainers) {
				yield return node.container;
			}
		}

		public void AddItemContainer(UIItemContainerNode node) => openContainers.Add(node);
		public void RemoveItemContainer(UIItemContainerNode node) => openContainers.Remove(node);

		bool IInputContext.HandleInput(in InputEvent inputEvent) {
			if (inputEvent.token.Equals(InputTokens.Mouse.position)) {
				pointerPosition = inputEvent.context.ReadValue<Vector2>();
				currentTransitStackHandle?.SetDisplayPosition(pointerPosition);
			}
			return false;
		}

		void IItemContainerScreenScope.SetTransitStackHandle(ITransitStackHandle transitStackHandle) {
			currentTransitStackHandle = transitStackHandle;
			transitStackHandle.SetDisplayParent(rect);
			transitStackHandle.SetDisplayPosition(pointerPosition);
		}

		void IItemContainerScreenScope.RemoveTransitStack() {
			currentTransitStackHandle?.Destroy();
			currentTransitStackHandle = null;
		}

		ItemStack? ITransitStackSource.GetTransitStack() => transitStack.GetStack();
		bool ITransitStackSource.HasTransitStack() => transitStack.HasStack();
		void ITransitStackSource.SetTransitStack(ItemStack? itemStack) {
			if (itemStack == null) transitStack.Release();
			else transitStack.SetStack(itemStack);
		}

		private void OnDestroy() {
			Soulbound.instance.GetInputManager().RemoveContext(this);
		}
	}
}
