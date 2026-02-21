using SoulboundBackend.Core.Debug.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Core.Debug.Logging.Logger;

namespace SoulboundBackend.Client.Combat {
	public sealed class Hurtbox : MonoBehaviour {
		[SerializeField] private ParticleSystem hitEffectPrefab;
#if UNITY_EDITOR
		[SerializeField] private new Collider2D collider;

		private void OnValidate() {
			if (collider == null) {
				return;
			}
			if (collider.gameObject != this.gameObject) {
				throw new ArgumentException($"Hurtbox collider not mapped to Hurtbox component on same game object! {gameObject}");
			}
			if (!collider.isTrigger) {
				throw new ArgumentException($"Hurtbox collider is has 'isTrigger' set to false");
			}
		}
#endif

		public void NotifyHit(AttackSource source) {
			Logger.LogInfo("hurtbox hit: "+ source.baseDamage);
			this.GetComponentInParent<Rigidbody2D>().AddForce(new Vector2(10f, 10f) * source.knockbackForce);
			GetComponentInParent<HitFlashEffect>().TriggerFlash();

			for (int i = 0; i < source.baseDamage; i++) {
				var ps = GameObject.Instantiate(hitEffectPrefab.gameObject).GetComponent<ParticleSystem>();
				ps.transform.position = transform.position + new Vector3(0f, 1f);
				ps.Play();
				Destroy(ps.gameObject, ps.main.duration + 0.1f);
			}
		}
	}
}
