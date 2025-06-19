using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatProvider : IItemCapability {
	public bool ApplyStatsAutomatically { get; }
	public List<SerializableStat> Stats { get; }

	// FEATUREIMPL: buffered stats
	// Currently stats are statically added, meaning they add to the total bonus. Potions or special
	// consumables may add stats but in a restricted manner - upon consume they only *reset* the
	// applied stats - not add to the total value. This is just one type of buffered stats

	public virtual void ApplyStats(PlayerController player) {
		player.Stats.Apply(Stats);
	}
}