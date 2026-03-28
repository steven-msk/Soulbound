using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Render;
using System;
#nullable enable

public class FakeItem : Item {
	public override string name => _name;
	private readonly string _name;
	public override int fullStackSize => _fullStackSize;
	public int _fullStackSize = Item.DEFAULT_FULL_STACK;

	public FakeItem() : base($"fakeItem_{Guid.NewGuid()}") {
		_name = this.GetID();
	}

	public FakeItem(string id) : base(id) {
		_name = id;
	}

	public override ItemRenderData GetRenderData(ItemStack itemStack) {
		return default;
	}
}
