using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	[PROTOTYPICAL]
	public static class AudioManager {
		public const float MASTER_VOLUME = 1f;
		private static readonly Dictionary<SoundType, float> typeVolumes = new() {
			[SoundType.SFX] = 1f,
			[SoundType.UI] = 1f,
			[SoundType.Music] = 1f,
			[SoundType.Ambient] = 1f
		};
		private static readonly Dictionary<SoundType, AudioSourcePool> sourcePools = new() {
			[SoundType.SFX] = new AudioSourcePool(16),
			[SoundType.UI] = new AudioSourcePool(8),
			[SoundType.Ambient] = new AudioSourcePool(4),
			[SoundType.Music] = new AudioSourcePool(2)
		};

		public static void RebuildPools() {
			foreach (var pool in sourcePools.Values) {
				pool.RebuildSources();
			}
		}

		public static void Play(AudioCue cue) {
			SoundDefinition sound = GetSound(cue);
			if (sound == null) return;

			SoundType soundType = sound.GetSoundType();

			AudioSourcePool sourcePool = sourcePools[soundType];
			AudioSource source = sourcePool.Get();

			float volume = cue.volumeOverride != null
				? cue.volumeOverride.Value
				: sound.GetVolume();

			// independent source play policy
			// this creates unresponsiveness when all sources
			// in the pool are playing
			if (!source.isPlaying) {
				source.clip = sound.GetClip();
				source.volume = volume * MASTER_VOLUME * typeVolumes[sound.GetSoundType()];
				source.pitch = sound.GetPitch();
				source.Play();
			}
		}

		private static SoundDefinition GetSound(AudioCue cue) {
			return AssetManager.Resolve<SoundDefinition>(cue.assetKey);
		}
	}
}
