using SoulboundEngine.Client.Input;
using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Container;
using SoulboundEngine.Client.ItemSystem.Container.View;
using SoulboundEngine.Client.Render.Item;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundEngine.Client.UI.Screen {
	[RequireComponent(typeof(RectTransform))]
	public class WorldSessionScreenObject : ScreenObject, IItemContainerScreenScope, IInputEventHandler {
		private RectTransform rect = null!;
		private TransitStack transitStack = null!;
		private SlotDragState? dragState;
		private readonly List<UIItemContainerNode> openContainers = new();
		private Vector2 pointerPosition;
		int IInputEventHandler.priority => 5000;

		public void Init(ItemRenderManager itemRenderManager, Screen screen) {
			base.Init(screen);
			this.rect = this.GetComponent<RectTransform>();
			this.transitStack = new TransitStack(itemRenderManager, this.rect);
			SoulboundClient.Instance.InputManager.AddHandler(this);
		}

		public bool TryBeginDrag(ItemStack stack, SlotRef slotRef, PointerEventData.InputButton button) {
			if (this.InDragState() || stack == null) return false;

			HashSet<SlotRef> draggedSlots = new(new SlotRef.EqualityComparer()) { slotRef };

			this.dragState = new SlotDragState(slotRef.container) {
				stack = stack.Clone(),
				origin = slotRef,
				draggedSlots = draggedSlots,
				button = button,
				quantitySnapshots = this.CreateQuantitySnapshots(),
			};
			return true;
		}

		private Dictionary<SlotRef, int> CreateQuantitySnapshots() {
			Dictionary<SlotRef, int> snapshots = new();

			foreach (var node in this.openContainers) {
				Dictionary<int, int> quantities = this.GetQuantitySnapshotForContainer(node.container);

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

		public void EndDrag() => this.dragState = null;

		public void ExtendDrag(SlotRef slotRef) {
			this.dragState?.ExtendDrag(slotRef);
		}

		public bool InDragState() => this.dragState != null;

		public SlotDragState? GetDragState() => this.dragState;

		public IEnumerable<IItemContainer> GetOpenContainers() {
			foreach (var node in this.openContainers) {
				yield return node.container;
			}
		}

		public void AddItemContainer(UIItemContainerNode node) => this.openContainers.Add(node);
		public void RemoveItemContainer(UIItemContainerNode node) => this.openContainers.Remove(node);

		IEnumerable<InputEventListener> IInputEventHandler.GetListeners() {
			yield return InputEventListener.ObserveAny(InputTokens.Mouse.position, inputEvent => {
				this.pointerPosition = inputEvent.context.ReadValue<Vector2>();
				this.transitStack.SetPointerPosition(this.pointerPosition);
			});
		}

		ItemStack? ITransitStackSource.GetTransitStack() => this.transitStack.GetStack();
		bool ITransitStackSource.HasTransitStack() => this.transitStack.HasStack();
		void ITransitStackSource.SetTransitStack(ItemStack? itemStack) {
			if (itemStack == null) this.transitStack.Destroy();
			else this.transitStack.SetStack(itemStack);
		}

		private void OnDestroy() {
			SoulboundClient.Instance.InputManager.RemoveHandler(this);
		}
	}
}
