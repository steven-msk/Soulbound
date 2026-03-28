using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;

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
			return new ItemRenderData(new AssetKey("idkwhatthisis"), itemStack);
		}
	}
}
