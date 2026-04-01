using SoulboundEngine.Client.ItemSystem;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Core.Registry;
using System;
#nullable enable

public class FakeItem : Item {
	public override string name => _name;
	private readonly string _name;
	public override int fullStackSize => _fullStackSize;
	public int _fullStackSize = Item.DEFAULT_FULL_STACK;

	public FakeItem() : base(new Identifier("soulbound_tests", new[] { $"fakeitem_{DateTime.Now.ToBinary()}" })) {
		_name = this.GetIdentifier().path;
	}

	public FakeItem(Identifier id) : base(id) {
		_name = id.path;
	}

	public override ItemRenderData GetRenderData(ItemStack itemStack) {
		return default;
	}
}
