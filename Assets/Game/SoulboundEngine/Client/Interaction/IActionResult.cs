using SoulboundEngine.Client.Item;

namespace SoulboundEngine.Client.Interaction {
	public interface IActionResult {
		public static readonly Fail FAIL = new();
		public static readonly Success SUCCESS = new(new ItemContext(null));
		public static readonly Pass PASS = new();

		public virtual bool IsAccepted() => false;

		public sealed record ItemContext(ItemStack? newHandStack);

		public sealed record Success(ItemContext itemContext) : IActionResult {
			bool IActionResult.IsAccepted() => true;
		}

		public sealed record Fail : IActionResult;

		public sealed record Pass : IActionResult;

		public sealed record PassToBlockAction : IActionResult;
	}
}
