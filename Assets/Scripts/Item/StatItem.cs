using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class StatItem : Item, IStatProvider {
	public abstract bool ApplyStatsAutomatically { get; }

	public abstract List<SerializableStat> Stats { get; }

	// POTENTIAL: Func<bool> revokePredicate for buffered stats

	protected override CompoundTooltip GetDefaultTooltip() => base.GetDefaultTooltip().Concat(Tooltip.Stats(Stats));
}