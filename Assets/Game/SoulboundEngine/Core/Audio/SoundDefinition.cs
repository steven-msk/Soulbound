using System;
using UnityEngine;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Core.Audio {
	[CreateAssetMenu(menuName = "Audio/Sound", fileName = "sound")]
	public class SoundDefinition : ScriptableObject {
		[SerializeField] private SoundType soundType;
		[SerializeField] private AudioClip[] clips;

		[Header("Volume")]
		[SerializeField, Range(0f, 1f)] private float volume = 1f;
		[SerializeField] private bool randomizeVolume;
		[SerializeField, Range(0f, 1f)] private float volumeMin = 0f;
		[SerializeField, Range(0f, 1f)] private float volumeMax = 1f;

		[Header("Pitch")]
		[SerializeField] private float pitch = 1f;
		[SerializeField] private bool randomizePitch;
		[SerializeField] private float pitchMin;
		[SerializeField] private float pitchMax;

		public AudioClip GetClip() {
			if (clips == null || clips.Length == 0) {
				Logger.LogWarning("SoundDefinition '{}' doesn't have any audio clips.", this.name);
				return null;
			}
			return clips[UnityEngine.Random.Range(0, clips.Length)];
		}

		public float GetVolume() {
			if (!randomizeVolume) return volume;
			return UnityEngine.Random.Range(volumeMin, volumeMax);
		}

		public float GetPitch() {
			if (!randomizePitch) return pitch;
			return UnityEngine.Random.Range(pitchMin, pitchMax);
		}

		public SoundType GetSoundType() => soundType;
	}
}
