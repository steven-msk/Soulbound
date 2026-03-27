using NSubstitute;
using NUnit.Framework;
using SoulboundEngine.Client.ItemSystem.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[TestFixture]
internal abstract class SlotOperationTest {
	protected const int DEFAULT_FULL_STACK = 256;
	protected IItemContainerScope scope;
	protected FakeItem fakeItem;
	protected ISlotOperation operation;

	[SetUp]
	public void Setup() {
		scope = Substitute.For<IItemContainerScope>();
		fakeItem = new FakeItem {
			_fullStackSize = DEFAULT_FULL_STACK
		};
	}
}
