using System.Collections.Generic;

namespace SoulboundEngine.Client.World.EntitySystem.Attribute {
	public interface INumericModifierTarget {
		IEnumerable<INumericModifier> Resolve(IReadOnlyList<INumericModifier> modifiers);
	}
}
