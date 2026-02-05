using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace SoulboundBackend.Client.UI {
	[PROTOTYPICAL]
	public class TooltipHandle : MonoBehaviour, ITooltipHandle {
		public bool isAlive { get; private set; } = true;

		public event Action? onDestroyed;

		void ITooltipHandle.Destroy() {
			if (!isAlive) return;

			onDestroyed?.Invoke();
			isAlive = false;
			Destroy(gameObject);
		}

	}
}
