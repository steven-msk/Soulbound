using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHandler : MonoBehaviour {
	[SerializeField] private Animator animator;
	[SerializeField] private string attackTrigger;
	[SerializeField] private string windupTrigger;
	[SerializeField] private float windupTime;
	[SerializeField] private float cooldown;
	private PlayerController player;
	private WeaponItem weapon;

	public void Init(PlayerController player, WeaponItem weapon) {
		this.player = player;
		this.weapon = weapon;
		animator.SetTrigger(windupTrigger);
		player.CanAttack = false;
		gameObject.SetActive(true);
		StartCoroutine(HandleAttack());
	}

	private IEnumerator HandleAttack() {
		yield return new WaitForSeconds(windupTime);
		animator.SetTrigger(attackTrigger);
	}

	public void FinishAttack() {
		StartCoroutine(WaitCooldown());
	}

	private IEnumerator WaitCooldown() {
		yield return new WaitForSeconds(cooldown);
		player.CanAttack = true;
		Destroy(transform.parent.gameObject);
	}
}
