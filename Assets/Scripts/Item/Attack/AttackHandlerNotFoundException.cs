using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHandlerNotFoundException : NullReferenceException {
	public AttackHandlerNotFoundException(WeaponItem weapon, GameObject attackObject)
		: base($"AttackHandler not found in chilren of attack prefab asset. Item name: {weapon.name}, attack prefab: {attackObject.name}") { }
}