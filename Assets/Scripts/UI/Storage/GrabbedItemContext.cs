using NUnit.Framework.Internal.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

public class GrabbedItemContext {
	public ItemDisplay? value { get; private set; }
	public IItemSlot? lastKnownSlot { get; private set; }

	public GrabbedItemContext(ItemDisplay? initialValue, IItemSlot? currentSlot) {
		this.value = initialValue;
		this.lastKnownSlot = currentSlot;
	}

	public void Set(ItemDisplay? value, IItemSlot? slot) {
		this.value = value;
		if (slot != null) {
			this.lastKnownSlot = slot;
			Debug.Log("lastKnownSlot: " + slot.index);
		}
	}
}
