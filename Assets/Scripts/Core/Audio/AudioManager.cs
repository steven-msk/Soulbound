using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	[PROTOTYPICAL]
	public static class AudioManager {
		public const float MASTER_VOLUME = 1f;
		private static AudioSource oneShotSource;
		private static readonly AssetKey jumpClip = new("jump");
		private static readonly AssetKey blockBreakClip = new("blockBreak");
		private static readonly AssetKey blockPlaceClip = new("blockPlace");

		public static void Init(AudioSource source) {
			oneShotSource = source;
		}

		public static void Emit(AudioCue cue) {
			SoundDefinition sound = GetSound(cue);
			if (sound == null) return;

			AudioClip clip = sound.GetClip();
			oneShotSource.PlayOneShot(clip, sound.GetVolume() * MASTER_VOLUME);
		}

		private static SoundDefinition GetSound(AudioCue cue) {
			return AssetManager.Resolve<SoundDefinition>(cue switch {
				AudioCue.BlockBreak => blockBreakClip,
				AudioCue.BlockPlace => blockPlaceClip,
				AudioCue.Jump => jumpClip,
				_ => null
			});
		}
	}
}
