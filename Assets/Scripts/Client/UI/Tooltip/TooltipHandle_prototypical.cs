using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public class TooltipHandle_prototypical : MonoBehaviour, ITooltipHandle {
		public bool isAlive { get; private set; } = true;
		public event Action onDestroyed;

		public void Destroy() {
			onDestroyed();
			isAlive = false;
			Destroy(gameObject);
		}

	}
}
