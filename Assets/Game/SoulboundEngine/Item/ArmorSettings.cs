namespace SoulboundEngine.Item {
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Entity.Attribute;
	using System;
	using System.Collections.Generic;

	public record ArmorSettings(Dictionary<ArmorType, int> baseDefense, Func<ArmorType, ItemAttributeModifiers.Builder, ItemAttributeModifiers.Builder> attributeModifiers) {
		public static readonly ArmorSettings WOOD = new(MakeDefense(1, 2, 1, 1), (t, b) => b);
		public static readonly ArmorSettings STONE = new(MakeDefense(2, 4, 3, 2), (t, b) => b
			.Add(Attributes.SPEED, new AttributeModifier(Identifier.Of("stone_" + t.GetSerializedName()), -0.1d, AttributeModifier.Operation.ADDITIVE_PERCENT), t.GetSlot())
		);

		public ItemAttributeModifiers CreateAttributes(ArmorType type) {
			int defense = this.baseDefense.GetValueOrDefault(type, 0);
			ItemAttributeModifiers.Builder builder = ItemAttributeModifiers.Create();
			Identifier modifierId = Identifier.Of(type.GetSerializedName());
			builder.Add(Attributes.ARMOR, new AttributeModifier(modifierId, defense, AttributeModifier.Operation.ADDITIVE), type.GetSlot());
			return this.attributeModifiers(type, builder).Build();
		}

		private static Dictionary<ArmorType, int> MakeDefense(int helmet, int chestplate, int leggings, int boots) {
			return new Dictionary<ArmorType, int>() {
				[ArmorType.HELMET] = helmet,
				[ArmorType.CHESTPLATE] = chestplate,
				[ArmorType.LEGGINGS] = leggings,
				[ArmorType.BOOTS] = boots
			};
		}
	}
}
