using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class StackItem : Item {
		public override string name => $"Stack Item: {fullStackSize}";

		public override int fullStackSize { get; }

		public StackItem(int fullStackSize)
			: base($"stackItem_{fullStackSize}") {
			this.fullStackSize = fullStackSize;
		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("idkwhatthisis", itemStack);
		}
	}
}
