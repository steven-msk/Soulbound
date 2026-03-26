using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	[PROTOTYPICAL]
	public static class AudioManager {
		public const float MASTER_VOLUME = 1f;
		private static AudioSource oneShotSource;

		public static void InitOneShot(AudioSource source) {
			oneShotSource = source;
		}

		public static void Play(AudioCue cue) {
			SoundDefinition sound = GetSound(cue);
			if (sound == null) return;

			AudioClip clip = sound.GetClip();
			oneShotSource.pitch = sound.GetPitch();
			float volume = cue.volumeOverride != null
				? cue.volumeOverride.Value
				: sound.GetVolume();
			oneShotSource.PlayOneShot(clip, volume * MASTER_VOLUME);
		}

		private static SoundDefinition GetSound(AudioCue cue) {
			return AssetManager.Resolve<SoundDefinition>(cue.assetKey);
		}
	}
}
