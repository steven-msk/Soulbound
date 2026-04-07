namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public class EntityAttribute {
		public double DefaultValue { get; }
		public IValueRule ValueRule { get; }

		public EntityAttribute(IValueRule valueRule, double defaultValue) {
			this.DefaultValue = defaultValue;
			this.ValueRule = valueRule;
		}

	}
}
