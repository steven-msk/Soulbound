namespace SoulboundEngine.Item {
	using SoulboundEngine.Serialization;
	using SoulboundEngine.World.Entity;
	using System;
	using System.Collections.Generic;

	public readonly struct ArmorType : IEquatable<ArmorType> {
		private static readonly Dictionary<string, ArmorType> BY_SERIALIZED_NAME = new();
		private static readonly Dictionary<EquipmentSlot, ArmorType> BY_SLOT = new();
		public static readonly Codec<ArmorType> CODEC = Codecs.STRING.FlatXmap(
			encode: t => t.serializedName,
			decode: s => BySerializedName(s) is { } type
				? DataResult<ArmorType>.Success(type)
				: DataResult<ArmorType>.Error($"Unknown armor type {s}")
		);
		public static readonly ArmorType HELMET = new(EquipmentSlot.HEAD, "helmet");
		public static readonly ArmorType CHESTPLATE = new(EquipmentSlot.CHEST, "chestplate");
		public static readonly ArmorType LEGGINGS = new(EquipmentSlot.LEGS, "leggings");
		public static readonly ArmorType BOOTS = new(EquipmentSlot.FEET, "boots");
		private readonly EquipmentSlot slot;
		private readonly string serializedName;

		public ArmorType(EquipmentSlot slot, string serializedName) {
			this.slot = slot;
			this.serializedName = serializedName;
			BY_SERIALIZED_NAME.Add(serializedName, this);
			BY_SLOT.Add(slot, this);
		}

		public EquipmentSlot GetSlot() => this.slot;

		public string GetSerializedName() => this.serializedName;

		public static ArmorType? BySerializedName(string name) {
			return BY_SERIALIZED_NAME.TryGetValue(name, out ArmorType armorType) ? armorType : null;
		}

		public static ArmorType? ByEquipmentSlot(EquipmentSlot slot) {
			return BY_SLOT.TryGetValue(slot, out ArmorType armorType) ? armorType : null;
		}

		public bool Equals(ArmorType other) {
			return other.slot.Equals(this.slot)
				&& other.serializedName == this.serializedName;
		}

		public override bool Equals(object obj) {
			return obj is ArmorType other && other.Equals(this);
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.serializedName, this.slot);
		}

		public override string ToString() => this.serializedName;
	}
}
