using SoulboundEngine.Client.ItemSystem;
using System;
#nullable enable

public class FakeItem : Item {
	public FakeItem(int fullStackSize) : base(Settings.Stackable($"fakeitem_{DateTime.Now.ToBinary()}", fullStackSize, _ => default)) {
	}
}
