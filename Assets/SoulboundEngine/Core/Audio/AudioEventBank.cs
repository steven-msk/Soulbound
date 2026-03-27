using SoulboundEngine.Core.Event;
using System;
using System.Collections.Generic;

namespace SoulboundEngine.Core.Audio {
	public abstract class AudioEventBank : IAudioEventBank {
		protected readonly List<object> listeners = new();

		public void Activate() {
			listeners.ForEach(EventBus.AddAllInterfaces);
		}

		public void Deactivate() {
			listeners.ForEach(EventBus.RemoveAllInterfaces);
		}

		protected virtual AudioEventListener<T> AddListener<T>(Func<AudioCue> cueSupplier) where T : struct, IGameEvent {
			AudioEventListener<T> listener = new(cueSupplier);
			listeners.Add(listener);
			return listener;
		}
	}
}
