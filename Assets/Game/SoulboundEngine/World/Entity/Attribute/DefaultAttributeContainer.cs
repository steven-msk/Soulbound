using SoulboundEngine.Registry;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.World.Entity.Attribute {
	public sealed class DefaultAttributeContainer {
		private readonly Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults = new();

		public DefaultAttributeContainer(Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults) {
			this.defaults = defaults;
		}

		public AttributeInstance CreateInstance(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
			bool hasDefault = this.defaults.TryGetValue(attribute, out var defaultInstance);

			IValueRule? valueRule = ruleOverride ?? (hasDefault ? defaultInstance.ruleOverride : attribute.GetValue().ValueRule);
			AttributeInstance overrideInstance = new(attribute, valueRule) {
				baseValue = hasDefault ? defaultInstance.baseValue : attribute.GetValue().DefaultValue
			};

			return overrideInstance;
		}

		public IEnumerable<DefaultAttributeInstance> GetEntries() => this.defaults.Values;

		public static Builder NewBuilder() => new();

		public record DefaultAttributeInstance(RegistryEntry<EntityAttribute> entry, double baseValue, IValueRule? ruleOverride);

		public class Builder {
			private readonly Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults = new();
			private bool unmodifiable;

			public Builder Add(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
				if (this.unmodifiable) throw new InvalidOperationException();

				this.defaults.Add(attribute, this.CheckedAdd(attribute, attribute.GetValue().DefaultValue, ruleOverride));
				return this;
			}

			public Builder Add(RegistryEntry<EntityAttribute> attribute, double baseValue, IValueRule? ruleOverride = null) {
				if (this.unmodifiable) throw new InvalidOperationException();

				DefaultAttributeInstance instance = this.CheckedAdd(attribute, baseValue, ruleOverride);
				this.defaults.Add(attribute, instance);

				return this;
			}

			private DefaultAttributeInstance CheckedAdd(RegistryEntry<EntityAttribute> attribute, double baseValue, IValueRule? ruleOverride = null) {
				DefaultAttributeInstance instance = new(attribute, baseValue, ruleOverride);
				return instance;
			}

			public DefaultAttributeContainer Build() {
				this.unmodifiable = true;
				return new DefaultAttributeContainer(this.defaults);
			}
		}
	}
}
