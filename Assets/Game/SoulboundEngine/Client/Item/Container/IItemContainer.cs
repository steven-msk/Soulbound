using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container {
	public interface IItemContainer {
		IItemSlot GetSlot(int index);
		IReadOnlyList<int> GetAllSlots();
		int GetSlotCount();
	}
}
