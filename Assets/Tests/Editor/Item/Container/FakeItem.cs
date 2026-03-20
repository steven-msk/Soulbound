using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Client.ItemSystem.View;
using System;
#nullable enable

public class FakeItem : Item {
	public override string name => _name;
	private readonly string _name;
	public override ItemAspect aspect => null;
	public override int fullStackSize => _fullStackSize;
	public int _fullStackSize = Item.DEFAULT_FULL_STACK;

	public FakeItem() : base($"fakeItem_{Guid.NewGuid()}") {
		_name = this.GetID();
	}
}
