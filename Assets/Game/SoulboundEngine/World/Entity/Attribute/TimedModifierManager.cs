namespace SoulboundEngine.World.Entity.Attribute {
	using SoulboundEngine.Registry;
	using System.Collections.Generic;

	// TEMPORARY IMPLEMENTATION
	public sealed class TimedModifierManager {
		private readonly Dictionary<Identifier, ModifierInstance> activeModifiers = new();
		private readonly AttributeMap attributes;

		public TimedModifierManager(AttributeMap attributes) {
			this.attributes = attributes;
		}

		public void Add(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, int durationTicks) {
			this.attributes.GetInstance(attribute)?.AddOrUpdateTransientModifier(modifier);
			this.activeModifiers[modifier.id] = new ModifierInstance(attribute, modifier, durationTicks);
		}

		public void Tick() {
			if (this.activeModifiers.Count == 0) return;

			List<Identifier> expired = new();
			foreach ((Identifier id, ModifierInstance instance) in new Dictionary<Identifier, ModifierInstance>(this.activeModifiers)) {
				int remaining = instance.remainingTicks - 1;
				if (remaining <= 0) {
					this.attributes.GetInstance(instance.attribute)?.RemoveModifier(id);
					expired.Add(id);
				} else {
					this.activeModifiers[id] = instance with { remainingTicks = remaining };
				}
			}
			foreach (Identifier id in expired) {
				this.activeModifiers.Remove(id);
			}
		}

		private record ModifierInstance(RegistryEntry<AttributeType> attribute, AttributeModifier modifier, int remainingTicks);
	}
}
