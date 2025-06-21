using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHandler : MonoBehaviour {
	[SerializeField] private Animator animator;
	[SerializeField] private string attackTrigger;
	[SerializeField] private string windupTrigger;
	private PlayerController player = null; 
	private WeaponItem weapon = null;
	private AttackHandlerEvents events = null;

	public void Init(PlayerController player, WeaponItem weapon, AttackHandlerEvents events) {
		this.player = player;
		this.weapon = weapon;
		this.events = events;
		events.Setup(this);
	}

	public void BeginAttack() {
		player.CanAttack = false;
		transform.parent.gameObject.SetActive(true);
		player.StartCoroutine(HandleAttack(weapon.WindupTime));
		player.StartCoroutine(WaitCooldown());
	}

	public void InvokeAnimationEvent(string name) => events.InvokeAnimationEvent(name, this);

	public void FinishAttack() => events.PostAttack(this);

	private void OnTriggerEnter2D(UnityEngine.Collider2D collider) {
		if (collider.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) {
			throw new Exception($"Unexpected collision callback between weapon hitbox '{gameObject.name}' and non-hurtbox collider '{collider.gameObject.name}'");
		}
		Debug.Log("hit");
	}

	private IEnumerator HandleAttack(float windup) {
		if (weapon == null || player == null) {
			throw new AttackHandlerInitializationException($"HandleAttack() in {gameObject.name}");
		}
		if (windup > 0) {
			animator.SetTrigger(windupTrigger);
			yield return new WaitForSeconds(weapon.WindupTime);
		}
		animator.SetTrigger(attackTrigger);
	}

	private IEnumerator WaitCooldown() {
		if (weapon == null || player == null) {
			throw new AttackHandlerInitializationException($"WaitCooldown() in {gameObject.name}");
		}
		yield return new WaitForSeconds(weapon.Cooldown);
		player.CanAttack = true;
	}

	public class AttackHandlerInitializationException : Exception {
		public AttackHandlerInitializationException(string caller)
			: base($"Attack Handler not initialized! You must call AttackHandler.HandlePreAttack() before handling an attack. ({caller})") { }
	}
}
