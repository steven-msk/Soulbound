using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Core {
	public sealed class SoulboundUpdateScheduler : MonoBehaviour {
		private static SoulboundUpdateScheduler instance;

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
			obj.AddComponent<SoulboundUpdateScheduler>();
		}
	}
}
