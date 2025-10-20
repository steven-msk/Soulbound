using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.EventSystems;
using static SoulboundBackend.Client.UI.Storage.InventoryController;

#nullable enable

namespace SoulboundBackend.Common.UI.Storage {
	public class DragHandler {
		public IItemSlot[] slots { get; private set; }
		public IItemSlot origin { get; private set; }
		public InterpretationProvider interpretationProvider { get; private set; }
		public Dictionary<IItemSlot, int> quantitySnapshots { get; private set; }
		public PointerEventData.InputButton dragButton { get; private set; }
		public int startDragStack { get; }
		private List<IItemSlot> draggedSlots;
		public IReadOnlyList<IItemSlot> DraggedSlots => draggedSlots.AsReadOnly();

		public DragHandler(IItemSlot origin, Func<IItemSlot[]> slotsSupplier, InterpretationProvider interpretationProvider,
						   Dictionary<IItemSlot, int> quantitySnapshots, PointerEventData.InputButton dragButton) {
			this.slots = slotsSupplier.Invoke();
			this.origin = origin;
			this.interpretationProvider = interpretationProvider;
			this.startDragStack = origin.ItemStack?.quantity ?? 0;
			this.draggedSlots = new List<IItemSlot>() { origin };
			this.quantitySnapshots = quantitySnapshots;
			this.dragButton = dragButton;
		}

		public bool ExecuteInterpretation(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
			InterpretationFunction? function = interpretationProvider.Invoke(this, slot, grabbedItem);
			if (function != null) {
				var method = function.Method;
				if (method.GetCustomAttribute<InterpretationFunctionCandidateAttribute>() != null) {
					this.AddDraggedSlot(slot);
				}
			}
			function?.Invoke(slot, grabbedItem);
			return function != null;
		}

		public bool AddDraggedSlot(IItemSlot slot, bool allowDuplicates = false, bool showTooltip = false) {
			if (draggedSlots.Contains(slot) && !allowDuplicates) {
				return false;
			}
			slot.showTooltip = showTooltip;
			draggedSlots.Add(slot);
			return true;
		}

		public void OnDragEnd() {
			draggedSlots.ForEach(slot => slot.showTooltip = true);
		}
	}
}
