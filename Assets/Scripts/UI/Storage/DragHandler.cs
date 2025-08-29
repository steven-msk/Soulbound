using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using static InventoryController;

#nullable enable

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
		this.startDragStack = origin.ItemStack!.quantity;
		this.draggedSlots = new List<IItemSlot>() { origin };
		this.quantitySnapshots = quantitySnapshots;
		this.dragButton = dragButton;
	}

	// POTENTIAL: move drag callbacks in handler context, possibly add OnDragStart and OnDragEnd callbacks

	public bool ExecuteInterpretation(IItemSlot slot, RefBox<ItemDisplay> grabbedItem) {
		InterpretationFunction? function = interpretationProvider.Invoke(this, slot, grabbedItem);
		function?.Invoke(slot, grabbedItem);
		return function != null;
	}

	public bool AddDraggedSlot(IItemSlot slot, bool allowDuplicates = false) {
		if (draggedSlots.Contains(slot) && allowDuplicates) {
			return false;
		}
		draggedSlots.Add(slot);
		return true;
	}
}
