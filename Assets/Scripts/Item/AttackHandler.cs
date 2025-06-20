using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHandler : MonoBehaviour {
	[SerializeField] private Animator animator;
	[SerializeField] private string attackTrigger;
	[SerializeField] private string windupTrigger;
	private PlayerController player = null;
	private WeaponItem weapon = null;

	public void Init(PlayerController player, WeaponItem weapon) {
		this.player = player;
		this.weapon = weapon;
		animator.SetTrigger(windupTrigger);
		player.CanAttack = false;
		gameObject.SetActive(true);
		StartCoroutine(HandleAttack());
	}

	public void FinishAttack() {
		Destroy(transform.parent.gameObject, weapon.Cooldown);
		gameObject.SetActive(false);
		player.StartCoroutine(WaitCooldown());
	}

	private IEnumerator HandleAttack() {
		if (weapon == null || player == null) {
			Debug.Log($"Attack Handler not initialized! HandleAttack() in ({gameObject.name})");
			yield break;
		}

		yield return new WaitForSeconds(weapon.WindupTime);
		animator.SetTrigger(attackTrigger);
	}

	private IEnumerator WaitCooldown() {
		if (weapon == null || player == null) {
			Debug.Log($"Attack Handler not initialized! WaitCooldown() in ({gameObject.name})");
			yield break;
		}

		yield return new WaitForSeconds(weapon.Cooldown);
		player.CanAttack = true;
	}
}
