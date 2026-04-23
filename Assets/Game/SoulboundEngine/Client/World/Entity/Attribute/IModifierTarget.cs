using System.Collections.Generic;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface IModifierTarget {
		IEnumerable<AttributeModifier> Resolve(IReadOnlyList<AttributeModifier> modifiers);
	}
}
