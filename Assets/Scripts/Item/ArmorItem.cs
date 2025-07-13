using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ArmorItem")]
public class ArmorItem : StatItem, IEquipable {
	[SerializeField] private ArmorType armorType;
	public ArmorType ArmorType => armorType;

	public override bool ApplyInstantStatsAutomatically => false;

	[SerializeField] private List<SerializableStat> instantStats;
	public override List<SerializableStat> InstantStats => instantStats;

	[SerializeField] private List<BufferedStat> bufferedStats;
	public override List<BufferedStat> BufferedStats => bufferedStats;

	[SerializeField] private string bufferedInterpolationSource;
	public override string BufferedInterpolationSource => bufferedInterpolationSource;

	public void OnEquip(EquipmentSlot slot) {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.Apply(instantStats, this);
		((IStatProvider)this).EnableBuffers(playerStats);
	}

	public void OnUnequipped() {
		PlayerStats playerStats = GameManager.instance.Player.Stats;
		playerStats.Revoke(instantStats, this);
		((IStatProvider)this).DisableBuffers(playerStats);
	}
}