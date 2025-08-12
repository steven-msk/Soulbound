using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public record DroppedItemSpawnData : EntitySpawnData {
	public ItemStack itemStack;
	public float pickupDelay;
	public Vector2 dropForce;

	public DroppedItemSpawnData(ItemStack itemStack, float pickupDelay, Vector2 pos, Vector2 dropForce) 
		: base(pos) {
		this.itemStack = itemStack;
		this.pickupDelay = pickupDelay;
		this.dropForce = dropForce;
	}
}
