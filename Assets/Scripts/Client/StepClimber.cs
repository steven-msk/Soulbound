using SoulboundBackend.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundBackend.Client {
	public class StepClimber : MonoBehaviour {
		[SerializeField] private float maxStepHeight = 1f;
		[SerializeField] private float stepSearchOvershoot = 0.1f;
		[SerializeField] private float smoothStepDuration = 0.05f;

		private PlayerController player;
		private Rigidbody2D rb;
		private List<ContactPoint2D> allContactPoints = new();
		private Vector2 lastVelocity;
		Coroutine stepCoroutine = null;

		private void Start() {
			player = LevelManager.instance.Player;
			rb = this.GetComponent<Rigidbody2D>();
		}

		private void FixedUpdate() {
			Vector2 velocity = rb.linearVelocity;
			ContactPoint2D ground = default(ContactPoint2D);

			bool grounded = FindGround(out ground, allContactPoints);
			Vector2 stepUpOffset = default(Vector2);
			bool stepUp = false;

			if (grounded && stepCoroutine == null) {
				stepUp = FindStep(out stepUpOffset, allContactPoints, ground, lastVelocity);
			}
			if (stepUp) {
				if (stepCoroutine != null) {
					StopCoroutine(stepCoroutine);
				}
				stepCoroutine = StartCoroutine(SmoothStepUp(stepUpOffset, lastVelocity));
			}

			allContactPoints.Clear();
			lastVelocity = velocity;
		}

		private IEnumerator SmoothStepUp(Vector2 stepUpOffset, Vector2 preserveVelocity) {
			Vector2 start = rb.position;
			Vector2 end = rb.position + stepUpOffset;
			float elapsed = 0;
			while (elapsed < smoothStepDuration) {
				elapsed += Time.fixedDeltaTime;
				rb.MovePosition(Vector2.Lerp(start, end, elapsed / smoothStepDuration));
				rb.linearVelocity = new Vector2(preserveVelocity.x, rb.linearVelocityY);
				yield return new WaitForFixedUpdate();
			}
			rb.position = end;
			rb.linearVelocity = preserveVelocity;
			stepCoroutine = null;
		}

		private void OnCollisionEnter2D(Collision2D collision) => allContactPoints.AddRange(collision.contacts);

		private void OnCollisionStay2D(Collision2D collision) => allContactPoints.AddRange(collision.contacts);

		bool FindGround(out ContactPoint2D ground, List<ContactPoint2D> allContactPoints) {
			ground = default(ContactPoint2D);
			bool found = false;
			foreach (ContactPoint2D contactPoint in allContactPoints) {
				if (contactPoint.normal.y > 0.5f && (found == false || contactPoint.normal.y > ground.normal.y)) {
					ground = contactPoint;
					found = true;
				}
			}
			return found;
		}

		bool FindStep(out Vector2 stepUpOffset, List<ContactPoint2D> allContactPoints, ContactPoint2D ground, Vector2 lastVelocity) {
			stepUpOffset = default(Vector2);
			float velocityX = lastVelocity.x;
			if (velocityX == 0) {
				return false;
			}
			foreach (ContactPoint2D contactPoint in allContactPoints) {
				bool test = ResolveStepUp(out stepUpOffset, contactPoint, ground);
				if (test) {
					return true;
				}
			}
			return false;
		}

		bool ResolveStepUp(out Vector2 stepUpOffset, ContactPoint2D stepTestContactPoint, ContactPoint2D ground) {
			stepUpOffset = default(Vector2);
			Collider2D stepCollider = stepTestContactPoint.otherCollider;
			if (stepTestContactPoint.normal.y >= 0.01f) {
				return false;
			}
			if (!(stepTestContactPoint.point.y - ground.point.y < this.maxStepHeight)) {
				return false;
			}

			float stepHeight = ground.point.y + maxStepHeight + 0.0001f;
			Vector2 stepTestInvDir = new Vector2(-stepTestContactPoint.normal.x, 0).normalized;
			Vector2 origin = new Vector2(stepTestContactPoint.point.x, stepHeight) + (stepTestInvDir * stepSearchOvershoot);
			RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, maxStepHeight);
			if (hit.collider == null) {
				return false;
			}
			Vector2 stepUpPoint = new Vector2(stepTestContactPoint.point.x, hit.point.y + 0.0001f) + (stepTestInvDir * stepSearchOvershoot);
			stepUpOffset = stepUpPoint - new Vector2(stepTestContactPoint.point.x, ground.point.y) + new Vector2(0.2f * player.facing, 0);
			return true;
		}
	}
}