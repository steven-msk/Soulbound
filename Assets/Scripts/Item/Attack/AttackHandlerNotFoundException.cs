using System;
using UnityEngine;

public class AttackHandlerNotFoundException : NullReferenceException {
	public AttackHandlerNotFoundException(WeaponItem weapon, GameObject attackObject)
		: base($"AttackHandler not found in chilren of attack prefab asset. Item name: {weapon.name}, attack prefab: {attackObject.name}") { }
}