namespace SoulboundEngine.Item {
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;

	// TEST ITEM
	public class JumpEffectItem : Item {
		public JumpEffectItem(Settings settings) 
			: base(settings) {
		}

		public override IActionResult OnSecondaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			player.AddTimedAttributeModifier(Attributes.JUMP_POWER, new AttributeModifier(Identifier.Of("jump_increase"), 2.0d, AttributeModifier.Operation.MULTIPLICATIVE), 500);
			return IActionResult.SUCCESS.DamageItem(player, EquipmentSlot.MAIN_HAND);
		}
	}
}
