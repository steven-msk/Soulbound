using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

public class ConsumableStatItem : Item, IConsumable, IStatProvider {
	[CanBeNull][SerializeField] private ConsumableEffect consumeAction;
	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;

	[SerializeField] private List<SerializableStat> stats;
	public List<SerializableStat> Stats => stats;

	public bool ApplyStatsAutomatically => false;
}
