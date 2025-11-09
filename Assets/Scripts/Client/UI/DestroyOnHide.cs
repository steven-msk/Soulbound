using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class DestroyOnHide : MonoBehaviour {
		private void OnDisable() => Destroy(gameObject);
	}
}
