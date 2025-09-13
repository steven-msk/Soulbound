using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CooldownRestriction : IConsumptionRestriction {
	private readonly float cooldown;
	private float lastConsumed = -999f;

	public CooldownRestriction(float cooldown) {
		this.cooldown = cooldown;
	}

	public bool CanConsume(IConsumable consumable, ItemStack itemStack) {
		return !(Time.time - lastConsumed < cooldown);
	}

	public void NotifyConsumed(ItemStack itemStack) {
		lastConsumed = Time.time;
	}
}
