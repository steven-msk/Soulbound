using System.Collections.Generic;

namespace SoulboundEngine.Client.World.Entity.Attribute {
	public interface IModifierTarget {
		IEnumerable<AttributeModifier> Resolve(IReadOnlyList<AttributeModifier> modifiers);
	}
}
