using System;
using System.Collections;
using UnityEngine;

public class AttackHandler : MonoBehaviour {

	// FIXME: inacurrate collision detection between attack hitbox and target hurtbox
	// To test this, set the cooldown of some fast attack to something very small (like 0.15f)
	// then repeatedly attack an enemy and count the logs of the attack and hit callbacks
	// the number of attack logs should be equal to the number of hit callbacks invoked
	// if thats not the case, youve got yourself a bug

	[SerializeField] private Animator animator;
	private PlayerController player = null;
	private WeaponItem weapon = null;
	private AttackHandlerEvents events = null;
	public GameObject Parent => transform.parent.gameObject;

	public void Init(WeaponItem weapon, AttackHandlerEvents events) {
		this.player = GameManager.instance.Player;
		this.weapon = weapon;
		this.events = events;
		events.PreAttack(Parent);
	}

	public void HandleAttack(AttackProcedure procedure) {
		if (weapon == null || player == null) {
			throw new AttackHandlerInitializationException($"BeginAttack() in '{gameObject.name}'");
		}
		player.CanAttack = false;
		Parent.SetActive(true);
		EventBus<GameEvent>.Publish(GameEvent.PlayerAttackStart);
		procedure.InvokeAnimation(animator);
		player.StartCoroutine(WaitCooldown(procedure.Cooldown));
	}

	public void InvokeAnimationEvent(string name) => events.InvokeAnimationEvent(name, Parent);

	internal void FinishAttack(string attack) { 
		EventBus<GameEvent>.Publish(GameEvent.PlayerAttackEnd);
		events.PostAttack(Parent, attack);
	}

	private void OnTriggerEnter2D(UnityEngine.Collider2D collider) {
		if (collider.gameObject.layer != LayerMask.NameToLayer("Hurtbox")) {
			throw new Exception($"Unexpected collision callback between weapon hitbox '{gameObject.name}' and non-hurtbox collider '{collider.gameObject.name}'");
		}
		// weapon.DealDamage(ILivingEntity entity)
		events.OnHit(Parent);
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
