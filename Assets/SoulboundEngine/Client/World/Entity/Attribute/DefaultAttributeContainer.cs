using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class DefaultAttributeContainer {
		private readonly Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults = new();

		public DefaultAttributeContainer(Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults) {
			this.defaults = defaults;
		}

		public AttributeInstance CreateInstance(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
			bool hasDefault = defaults.TryGetValue(attribute, out var defaultInstance);

			IValueRule? valueRule = ruleOverride ?? (hasDefault ? defaultInstance.ruleOverride : attribute.GetValue().ValueRule);
			AttributeInstance overrideInstance = new(attribute, valueRule) {
				baseValue = hasDefault ? defaultInstance.baseValue : attribute.GetValue().DefaultValue
			};

			return overrideInstance;
		}

		public IEnumerable<DefaultAttributeInstance> GetEntries() => defaults.Values;

		public static Builder NewBuilder() => new();

		public record DefaultAttributeInstance(RegistryEntry<EntityAttribute> entry, double baseValue, IValueRule? ruleOverride);

		public class Builder {
			private readonly Dictionary<RegistryEntry<EntityAttribute>, DefaultAttributeInstance> defaults = new();
			private bool unmodifiable;

			public Builder Add(RegistryEntry<EntityAttribute> attribute, IValueRule? ruleOverride = null) {
				if (unmodifiable) throw new InvalidOperationException();

				defaults.Add(attribute, CheckedAdd(attribute, attribute.GetValue().DefaultValue, ruleOverride));
				return this;
			}

			public Builder Add(RegistryEntry<EntityAttribute> attribute, double baseValue, IValueRule? ruleOverride = null) {
				if (unmodifiable) throw new InvalidOperationException();

				DefaultAttributeInstance instance = CheckedAdd(attribute, baseValue, ruleOverride);
				defaults.Add(attribute, instance);

				return this;
			}

			private DefaultAttributeInstance CheckedAdd(RegistryEntry<EntityAttribute> attribute, double baseValue, IValueRule? ruleOverride = null) {
				DefaultAttributeInstance instance = new(attribute, baseValue, ruleOverride);
				return instance;
			}

			public DefaultAttributeContainer Build() {
				unmodifiable = true;
				return new DefaultAttributeContainer(defaults);
			}
		}
	}
}
