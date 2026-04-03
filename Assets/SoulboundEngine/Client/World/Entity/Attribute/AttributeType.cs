using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public abstract class AttributeType<T> : AttributeType {
		protected new readonly IValueRule<T>? valueRule;

		public AttributeType(Identifier identifier, IValueRule<T>? valueRule)
			: base(identifier, valueRule) {
			this.valueRule = valueRule;
		}

		public abstract T ComputeValue(T baseValue, IValueRule<T>? ruleOverride, IReadOnlyList<IAttributeModifier<T>> modifiers, IAttributeContext context);

		public override object ComputeValue(object baseValue, IValueRule? ruleOverride, IReadOnlyList<IAttributeModifier> modifiers, IAttributeContext context) {
			if (baseValue is not T) {
				Logger.LogFatal(ReceivedInvalidComputeValueType(baseValue.GetType().Name));
				baseValue = default(T);
			}

			if (ruleOverride != null) {
				if (ruleOverride is not IValueRule<T>) {
					string types = string.Join(", ", ruleOverride.GetType().GetGenericArguments().Select(t => t.Name));
					Logger.LogFatal(ReceivedInvalidComputeValueType(types));
					ruleOverride = null;
				}
			}

			List<IAttributeModifier<T>> castModifiers = new();
			foreach (var modifier in modifiers) {
				if (modifier is not IAttributeModifier<T> cast) {
					string types = string.Join(", ", modifier.GetType().GetGenericArguments().Select(t => t.Name));
					Logger.LogFatal(ReceivedInvalidComputeValueType(types));
				} else {
					castModifiers.Add(cast);
				}
			}

			return ComputeValue((T)baseValue, (IValueRule<T>?)ruleOverride, castModifiers, context);
		}

		public new IValueRule<T>? GetValueRule() => (IValueRule<T>?)valueRule;

		private InvalidOperationException ReceivedInvalidComputeValueType(string received) {
			return new InvalidOperationException($"ComputeValue expected {typeof(T).Name} but received {received}.");
		}
	}

	public abstract class AttributeType : IIdentifierProvider {
		protected readonly Identifier identifier;
		protected readonly IValueRule? valueRule;

		public AttributeType(Identifier identifier, IValueRule? valueRule) {
			this.identifier = identifier;
			this.valueRule = valueRule;
		}

		public Identifier GetIdentifier() => identifier;
		public IValueRule? GetValueRule() => valueRule;

		public abstract object ComputeValue(object baseValue, IValueRule? ruleOverride, IReadOnlyList<IAttributeModifier> modifiers, IAttributeContext context);
	}
}
