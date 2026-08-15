using System.Collections.Generic;

namespace SoulboundEngine.World.Entity.Attribute {
	public interface IModifierTarget {
		IEnumerable<AttributeModifier> Resolve(IReadOnlyList<AttributeModifier> modifiers);
	}
}
