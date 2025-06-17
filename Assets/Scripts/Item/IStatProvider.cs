using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IStatProvider : IItemCapability {
	public bool ApplyStatsAutomatically { get; }
	public List<SerializableStat> Stats { get; }

#warning REMINDER currently stats are statically added, meaning they add to the total bonus. potions or special consumables may add stats but in a restricted manner - upon consume they only *reset* the applied stats - not add to the total value

	public virtual void ApplyStats(PlayerController player) {
		player.Stats.Apply(Stats);
	}
}