using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundEngine.Client.Stats {
	public sealed record StatModificationPackage(IStatDefinition definition, IReadOnlyList<IStatEntryModifier> modifiers);

	public interface IStatModificationSource {
		ModificationToken token { get; }
		IEnumerable<StatModificationPackage> GetPackages();
	}
}
