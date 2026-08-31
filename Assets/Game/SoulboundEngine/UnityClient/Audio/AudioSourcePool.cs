namespace SoulboundEngine.UnityClient.Audio {
	using UnityEngine;

	public class AudioSourcePool {
		private readonly AudioSource[] sources;
		private int nextIndex;

		public AudioSourcePool(int size) {
			this.sources = new AudioSource[size];
		}

		public AudioSource Get() {
			AudioSource source = this.sources[this.nextIndex];
			this.nextIndex = (this.nextIndex + 1) % this.sources.Length;
			return source;
		}

		public void RebuildSources() {
			for (int i = 0; i < this.sources.Length; i++) {
				if (this.sources[i] == null) continue;

				if (!this.sources[i]) {
					GameObject.Destroy(this.sources[i].gameObject);
				}
			}

			for (int i = 0; i < this.sources.Length; i++) {
				GameObject obj = new("Audio Source", typeof(AudioSource));
				this.sources[i] = obj.GetComponent<AudioSource>();
			}
		}
	}
}
