using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Graphs;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

#nullable enable

namespace SoulboundBackend.Client.UI {
	public class WorldSessionScreenObject : ScreenObject, IWorldSessionScreenObject {
		private TransitStack transitStack = null!;
		private SlotDragState? dragState;
		private readonly List<UIItemContainerNode> openContainers = new();
		TransitStack IItemContainerScope.transitStack => transitStack;

		public new void Init(Screen screen) {
			base.Init(screen);
			this.transitStack = new TransitStack(this);
		}

		public void AddItemContainer(UIItemContainerNode node) => openContainers.Add(node);

		public bool TryBeginDrag(IItemContainer container, int originSlotIndex, PointerEventData.InputButton button) {
			if (InDragState()) return false;

			IItemSlot originSlot = container.GetSlot(originSlotIndex);
			SlotRef originRef = new(container, originSlotIndex);
			SortedSet<SlotRef> draggedSlots = new(new SlotRef.Comparer()) { originRef };

			dragState = new SlotDragState(container) {
				item = originSlot.GetStack()!.item,
				origin = originRef,
				draggedSlots = draggedSlots,
				button = button,
				quantitySnapshots = CreateQuantitySnapshots(),
				originStack = originSlot.GetStack()!.quantity
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
			return container.GetAllSlots_indexed()
					.Where(i => container.GetSlot(i).GetStack()?.quantity > 0)
					.ToDictionary(i => i, i => container.GetSlot(i).GetStack()!.quantity);
		}

		public void EndDrag() => dragState = null;

		public void ExtendDrag(IItemContainer container, int slotIndex) {
			dragState?.AddDraggedSlot(container, slotIndex);
		}

		public bool InDragState() => dragState != null;

		public SlotDragState? GetDragState() => dragState;

		public IEnumerable<IItemContainer> GetOpenContainers() {
			foreach (var node in openContainers) {
				yield return node.container;
			}
		}
	}
}
