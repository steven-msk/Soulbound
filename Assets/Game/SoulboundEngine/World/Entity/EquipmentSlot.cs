namespace SoulboundEngine.World.Entity {
	using SoulboundEngine.Item;
	using System;
	using System.Collections.Generic;

	public readonly struct EquipmentSlot : IEquatable<EquipmentSlot> {
		public static readonly EquipmentSlot MAIN_HAND = new("main_hand", 0, 1);
		public static readonly IEnumerable<EquipmentSlot> VALUES = new[] {
			MAIN_HAND
		};
		private readonly string serializedName;
		private readonly int id;
		private readonly int countLimit;

		public EquipmentSlot(string name, int countLimit, int id) {
			this.countLimit = countLimit;
			this.serializedName = name;
			this.id = id;
		}

		public string GetSerializedName() => this.serializedName;

		public ItemStack Limit(ItemStack stack) {
			return this.countLimit > 0 ? stack.Split(this.countLimit) : stack;
		}

		public int GetId() => this.id;

		public static bool operator ==(EquipmentSlot a, EquipmentSlot b) => a.Equals(b);
		
		public static bool operator !=(EquipmentSlot a, EquipmentSlot b) => !a.Equals(b);

		public override bool Equals(object obj) {
			return obj is EquipmentSlot other && other.Equals(this);
		}

		public bool Equals(EquipmentSlot other) {
			return other.serializedName == this.serializedName
				&& other.id == this.id
				&& other.countLimit == this.countLimit;
		}

		public override int GetHashCode() {
			return HashCode.Combine(this.serializedName, this.id, this.countLimit);
		}

		public override string ToString() => this.serializedName;
	}
}
