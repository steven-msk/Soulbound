using SoulboundBackend.Core.Event;
using System;

namespace SoulboundBackend.Core.Audio {
	public class AudioEventListener<T> : IEventListener<T> where T : struct, IGameEvent {
		private readonly Func<AudioCue> cueSupplier;

		public AudioEventListener(Func<AudioCue> cueSupplier) {
			this.cueSupplier = cueSupplier;
		}

		public void OnEvent(T e) {
			AudioManager.Play(cueSupplier());
		}
	}
}
