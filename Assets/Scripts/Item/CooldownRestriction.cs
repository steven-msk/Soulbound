using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CooldownRestriction : IConsumptionRestriction, IStaticResettable {
	private readonly float cooldown;
	private float lastConsumed = float.NegativeInfinity;

	public CooldownRestriction(float cooldown) {
		this.cooldown = cooldown;
		StaticResetManager.Register(this);
	}

	public bool CanConsume(IConsumable consumable, ItemStack itemStack) {
		Debug.Log(lastConsumed);
		return !(Time.timeSinceLevelLoad - lastConsumed < cooldown);
	}

	public void NotifyConsumed(ItemStack itemStack) {
		lastConsumed = Time.timeSinceLevelLoad;
	}

	public void StaticReset() => lastConsumed = float.NegativeInfinity;
}
