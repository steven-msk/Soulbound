using SoulboundBackend.Common;
using SoulboundBackend.Core.Assets;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	[PROTOTYPICAL]
	public static class AudioManager {
		private static AudioSource oneShotSource;
		private static readonly AssetKey jumpClip = new("jump");
		private static readonly AssetKey blockBreakClip = new("blockBreak");
		private static readonly AssetKey blockPlaceClip = new("blockPlace");

		public static void Init(AudioSource source) {
			oneShotSource = source;
		}

		public static void Emit(AudioCue cue) {
			AudioClip clip = GetClip(cue);
			if (clip == null) return;
			oneShotSource.PlayOneShot(clip);
		}

		private static AudioClip GetClip(AudioCue cue) {
			return AssetManager.Resolve<AudioClip>(cue switch {
				AudioCue.BlockBreak => blockBreakClip,
				AudioCue.BlockPlace => blockPlaceClip,
				AudioCue.Jump => jumpClip,
				_ => null
			});
		}
	}
}
