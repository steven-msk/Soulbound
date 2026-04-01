using System.Collections.Generic;

#nullable enable

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public sealed class AttributeContainer {
		private readonly IReadOnlyDictionary<AttributeType, object> baseValues;
		private readonly IReadOnlyDictionary<AttributeType, IValueRange?> rangeOverrides;

		public AttributeContainer(
				IReadOnlyDictionary<AttributeType, object> baseValues,
				IReadOnlyDictionary<AttributeType, IValueRange?> rangeOverrides
			) {
			this.baseValues = baseValues;
			this.rangeOverrides = rangeOverrides;
		}
	}
}
