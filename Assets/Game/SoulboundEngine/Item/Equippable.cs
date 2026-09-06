namespace SoulboundEngine.Item {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity;

	public record Equippable(EquipmentSlot slot) {
		public static readonly Codec<Equippable> CODEC = EquipmentSlot.CODEC.Xmap(s => new Equippable(s), e => e.slot);
	}
}
