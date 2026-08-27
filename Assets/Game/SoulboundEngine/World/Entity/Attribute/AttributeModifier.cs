namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;

#nullable enable

	public record AttributeModifier(Identifier id, double amount, AttributeModifier.Operation operation) {
		public bool Matches(Identifier id) => id.Equals(this.id);

		public readonly struct Operation {
			public static readonly Operation ADDITIVE = new("additive", 0);                 // +A  or -A
			public static readonly Operation ADDITIVE_PERCENT = new("additive_percent", 1); // +B% or -B%
			public static readonly Operation MULTIPLICATIVE = new("multiplicative", 2);     // xC  or x1/C
			public readonly string serializedName;
			public readonly int id;

			private Operation(string name, int id) {
				this.serializedName = name;
				this.id = id;
			}
		}
	}
}
