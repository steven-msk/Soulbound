namespace SoulboundEngine.Interaction {
	using SoulboundEngine.Item;
	using SoulboundEngine.World.Entity;
	using System;

	public interface IActionResult {
		public static readonly Fail FAIL = new();
		public static readonly Success SUCCESS = new(s => s);
		public static readonly Pass PASS = new();
		public static readonly PassToBlockAction PASS_TO_BLOCK_ACTION = new();

		public virtual ItemStack ReplaceStack(ItemStack stack) {
			return stack;
		}

		public virtual bool HasAction() => false;

		public sealed record Success(Func<ItemStack, ItemStack> stackReplacer) : IActionResult {
			public Success DamageItem(Entity entity, EquipmentSlot slot, int amount = 1) {
				return new Success(stack => {
					stack.DamageAndBreak(amount, entity, slot);
					return stack;
				});
			}

			public Success DecrementStack() {
				return new Success(stack => stack.DecrementBy(1));
			}

			ItemStack IActionResult.ReplaceStack(ItemStack stack) => this.stackReplacer(stack);

			bool IActionResult.HasAction() => true;
		}

		public sealed record Fail : IActionResult {
			bool IActionResult.HasAction() => true;
		}

		public sealed record Pass : IActionResult;

		public sealed record PassToBlockAction : IActionResult;
	}
}
