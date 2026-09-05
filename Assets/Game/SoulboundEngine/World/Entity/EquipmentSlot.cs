namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Item;
	using SoulboundEngine.Serialization;
	using System;
	using System.Collections.Generic;

	public readonly struct EquipmentSlot : IEquatable<EquipmentSlot> {
		public static readonly Codec<EquipmentSlot> CODEC = Codecs.STRING.FlatXmap(
			encode: s => s.serializedName,
			decode: s => BySerializedName(s) is { } slot
				? DataResult<EquipmentSlot>.Success(slot)
				: DataResult<EquipmentSlot>.Error($"Invalid equipment slot: {s}")
		);
		private static readonly Dictionary<string, EquipmentSlot> BY_SERIALIZED_NAME = new();
		public static readonly EquipmentSlot MAIN_HAND = new("main_hand", 0);
		public static readonly EquipmentSlot HEAD = new("head", 1);
		public static readonly EquipmentSlot CHEST = new("chest", 1);
		public static readonly EquipmentSlot LEGS = new("legs", 1);
		public static readonly EquipmentSlot FEET = new("feet", 1);
		public static readonly IEnumerable<EquipmentSlot> VALUES = new[] {
			MAIN_HAND, HEAD, CHEST, LEGS
		};
		private readonly string serializedName;
		private readonly int countLimit;

		public EquipmentSlot(string name, int countLimit) {
			this.countLimit = countLimit;
			this.serializedName = name;
			BY_SERIALIZED_NAME.Add(name, this);
		}

		public string GetSerializedName() => this.serializedName;

		public static EquipmentSlot? BySerializedName(string name) {
			return BY_SERIALIZED_NAME.TryGetValue(name, out EquipmentSlot slot) ? slot : null;
		}

		public ItemStack Limit(ItemStack stack) {
			return this.countLimit > 0 ? stack.Split(this.countLimit) : stack;
		}

		public static bool operator ==(EquipmentSlot a, EquipmentSlot b) => a.Equals(b);
		
		public static bool operator !=(EquipmentSlot a, EquipmentSlot b) => !a.Equals(b);

		public override bool Equals(object obj) {
			return obj is EquipmentSlot other && other.Equals(this);
		}

		public bool Equals(EquipmentSlot other) {
			return other.serializedName == this.serializedName
				&& other.countLimit == this.countLimit;
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.serializedName, this.countLimit);
		}

		public override string ToString() => this.serializedName;
	}
}
