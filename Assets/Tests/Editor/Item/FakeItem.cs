using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Render;
using System;
#nullable enable

public class FakeItem : Item {
	public override string name => _name;
	private readonly string _name;
	public override int fullStackSize => _fullStackSize;
	public int _fullStackSize = Item.DEFAULT_FULL_STACK;

	public FakeItem() : base() {
		_name = $"fakeitem_{DateTime.Now.ToBinary()}";
	}

	public override ItemRenderData GetRenderData(ItemStack itemStack) {
		return default;
	}
}
