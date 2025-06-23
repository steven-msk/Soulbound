using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using UnityEngine;

public abstract class ConsumableEffect : ItemUseEffect {
	public abstract void OnConsume(IConsumable consumable, ItemStack itemStack);

	// FEATUREIMPL: status effects
}
