using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem.Container;

[TestFixture]
internal abstract class SlotOperationTest {
	protected const int DEFAULT_FULL_STACK = 256;
	protected IInventoryScope scope;
	protected FakeItem fakeItem;
	protected ISlotOperation operation;

	[SetUp]
	public void Setup() {
		this.scope = Substitute.For<IInventoryScope>();
		this.fakeItem = new FakeItem(DEFAULT_FULL_STACK);
	}
}
