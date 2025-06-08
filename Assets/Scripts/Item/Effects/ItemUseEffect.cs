using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class ItemUseEffect : ScriptableObject {
	public abstract void Execute(ItemStack itemStack, PlayerController player);
}