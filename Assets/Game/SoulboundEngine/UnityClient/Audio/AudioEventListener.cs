using SoulboundEngine.Event;
using System;

namespace SoulboundEngine.UnityClient.Audio {
	public class AudioEventListener<T> : IEventListener<T> where T : struct, IGameEvent {
		private readonly Func<AudioCue> cueSupplier;

		public AudioEventListener(Func<AudioCue> cueSupplier) {
			this.cueSupplier = cueSupplier;
		}

		public virtual void OnEvent(T e) {
			AudioManager.Play(cueSupplier());
		}
	}
}
