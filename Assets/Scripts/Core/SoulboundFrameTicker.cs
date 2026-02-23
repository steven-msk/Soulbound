using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Core {
	public sealed class SoulboundFrameTicker : MonoBehaviour {
		private static SoulboundFrameTicker instance;

		private void Awake() {
			if (instance != null) {
				Destroy(gameObject);
				return;
			}
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		private void Update() {
			Soulbound.instance?.FrameTick();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap() {
			GameObject obj = new("SoulboundFrameTicker");
			obj.AddComponent<SoulboundFrameTicker>();
		}
	}
}
