using SoulboundBackend.Client;
using SoulboundBackend.Core.Assets;
using SoulboundBackend.Core.Event;
using System;
using System.Collections.Generic;

namespace SoulboundBackend.Core.Audio {
	public class AudioEventBank : IAudioEventBank {
		private static readonly AssetKey blockBreak = new("blockBreak");
		private static readonly AssetKey blockPlace = new("blockPlace");
		private static readonly AssetKey jump = new("jump");
		private readonly List<object> listeners = new();

		public AudioEventBank() {
			AddListener<BlockBrokenEvent>(() => new AudioCue(blockBreak));
			AddListener<BlockPlacedEvent>(() => new AudioCue(blockPlace));
			AddListener<PlayerJumpedEvent>(() => new AudioCue(jump));
		}

		public void Activate() {
			foreach (var listener in listeners) {
				EventBus.AddAllInterfaces(listener);
			}
		}

		public void Deactivate() {
			foreach (var listener in listeners) {
				EventBus.RemoveAllInterfaces(listener);
			}
		}

		private AudioEventListener<T> AddListener<T>(Func<AudioCue> cueSupplier) where T : struct, IGameEvent {
			AudioEventListener<T> listener = new(cueSupplier);
			listeners.Add(listener);
			return listener;
		}

	}
}
