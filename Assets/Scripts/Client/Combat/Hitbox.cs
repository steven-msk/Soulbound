using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = SoulboundBackend.Common.Logging.Logger;

namespace SoulboundBackend.Client.Combat {
	[RequireComponent(typeof(Collider2D))]
	public class Hitbox : MonoBehaviour {
		private static readonly Logger logger = Logger.CreateInstance();
		[SerializeField] private new Collider2D collider;
		private AttackEventDispatcher owner;

#if UNITY_EDITOR
		private bool flag_invalidState = false;

		private void OnValidate() {
			if (!collider?.isTrigger ?? false && !flag_invalidState) {
				logger.LogError(null, "Hitbox collider is not set to trigger collider on {}", gameObject.name);
				flag_invalidState = true;
			} else {
				flag_invalidState = false;
			}
		}
#endif
		private void Update() {
			if (owner != null) {
				UnityEngine.Debug.Log("active: "+ collider.enabled);
			}
		}

		public void Activate(AttackEventDispatcher owner) {
			this.owner = owner;
			collider.enabled = true;
		}

		public void Deactivate() {
			collider.enabled = false;
			owner = null;
		}

		private void OnTriggerEnter2D(Collider2D other) {
			owner?.OnHitboxEnter(other);
			owner?.OnHitFrame(other);
		}

		private void OnTriggerStay2D(Collider2D other) {
			owner?.OnHitFrame(other);
		}

		private void OnTriggerExit2D(Collider2D other) {
			owner?.OnHitboxExit(other);
		}
	}
}
