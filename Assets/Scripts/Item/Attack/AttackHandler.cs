using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

	// FIXME: inacurrate collision detection between attack hitbox and target hurtbox

	[SerializeField] private Animator animator;
	private PlayerController player = null;
	private WeaponItem weapon = null;
	private AttackHandlerEvents events = null;

	public void Init(WeaponItem weapon, AttackHandlerEvents events) {
		this.player = GameManager.GetPlayerInstance();
		this.weapon = weapon;
		this.events = events;
		events.PreAttack(this);
	}

	public void HandleAttack(AttackProcedure procedure) {
		if (weapon == null || player == null) {
			throw new AttackHandlerInitializationException($"BeginAttack() in '{gameObject.name}'");
		}
		player.CanAttack = false;
		transform.parent.gameObject.SetActive(true);
		//player.StartCoroutine(HandleAttack(attackProcedure));
		procedure.InvokeAnimation(animator);
		player.StartCoroutine(WaitCooldown(procedure.Cooldown));
	}

	public void InvokeAnimationEvent(string name) => events.InvokeAnimationEvent(name, this);

	public void FinishAttack() => events.PostAttack(this);

	private void OnTriggerEnter2D(UnityEngine.Collider2D collider) {
		if (collider.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) {
			throw new Exception($"Unexpected collision callback between weapon hitbox '{gameObject.name}' and non-hurtbox collider '{collider.gameObject.name}'");
		}
		// weapon.DealDamage(ILivingEntity entity)
		events.OnHit(this);
	}

	private IEnumerator WaitCooldown(float cooldown) {
		if (weapon == null || player == null) {
			throw new AttackHandlerInitializationException($"WaitCooldown() in {gameObject.name}");
		}
		yield return new WaitForSeconds(cooldown);
		player.CanAttack = true;
	}

	public class AttackHandlerInitializationException : Exception {
		public AttackHandlerInitializationException(string caller)
			: base($"Attack Handler not initialized! You must call AttackHandler.Init() before handling an attack. ({caller})") { }
	}
}
