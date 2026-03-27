using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	public class AudioSourcePool {
		private readonly AudioSource[] sources;
		private int nextIndex;

		public AudioSourcePool(int size) {
			sources = new AudioSource[size];
		}

		public AudioSource Get() {
			AudioSource source = sources[nextIndex];
			nextIndex = (nextIndex + 1) % sources.Length;
			return source;
		}

		public void RebuildSources() {
			for (int i = 0; i < sources.Length; i++) {
				if (sources[i] == null) continue;

				if (!sources[i].IsDestroyed()) {
					GameObject.Destroy(sources[i].gameObject);
				}
			}

			for (int i = 0; i < sources.Length; i++) {
				GameObject obj = new("Audio Source", typeof(AudioSource));
				sources[i] = obj.GetComponent<AudioSource>();
			}
		}
	}
}
