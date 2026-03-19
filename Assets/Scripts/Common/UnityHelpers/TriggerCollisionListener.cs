using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Common.Unity {
	public class TriggerCollisionListener : MonoBehaviour {
		public event Action<Collider2D> onTriggerEnter;
		public event Action<Collider2D> onTriggerExit;

		private void OnTriggerEnter2D(Collider2D collision) {
			onTriggerEnter?.Invoke(collision);
		}

		private void OnTriggerExit2D(Collider2D collision) {
			onTriggerExit?.Invoke(collision);
		}
	}
}
