using SoulboundEngine.Client.ItemSystem;
using System;
#nullable enable

public class FakeItem : Item {
	public FakeItem(int fullStackSize) : base(Settings.Of($"fakeitem_{DateTime.Now.ToBinary()}").StackUpTo(fullStackSize)) {
	}
}
