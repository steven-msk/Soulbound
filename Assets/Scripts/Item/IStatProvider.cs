using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IStatProvider : IItemCapability {
	public bool ApplyStatsAutomatically { get; }
	public List<SerializableStat> Stats { get; }

	// FEATUREIMPL: buffered stats
	// Currently stats are statically added, meaning they add to the total bonus. Potions or special
	// consumables may add stats but in a restricted manner - upon consume they only *reset* the
	// applied stats - not add to the total value. This is just one type of buffered stats

	public virtual void ApplyStats(PlayerStats playerStats) => playerStats.Apply(Stats, this);

	public virtual void RevokeStats(PlayerStats platerStats) => platerStats.Revoke(Stats, this);
}