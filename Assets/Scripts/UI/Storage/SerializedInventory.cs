using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct SerializedInventory {
	public int lastHotbarIndex;
	public Dictionary<string, List<SerializedItemSlot>> regions;

	public SerializedInventory(int lastHotbarIndex, Dictionary<string, List<SerializedItemSlot>> regions) {
		this.lastHotbarIndex = lastHotbarIndex;
		this.regions = regions;
	}
}
