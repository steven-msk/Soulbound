namespace SoulboundEngine.Client.Audio {
	using System;
	using UnityEngine;

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
			if (this.clips == null || this.clips.Length == 0) {
				SoulboundEngine.Logger.LogWarning("SoundDefinition '{}' doesn't have any audio clips.", this.name);
				return null;
			}
			return this.clips[UnityEngine.Random.Range(0, this.clips.Length)];
		}

		public float GetVolume() {
			return !this.randomizeVolume ? this.volume : UnityEngine.Random.Range(this.volumeMin, this.volumeMax);
		}

		public float GetPitch() {
			return !this.randomizePitch ? this.pitch : UnityEngine.Random.Range(this.pitchMin, this.pitchMax);
		}

		public SoundType GetSoundType() => this.soundType;
	}
}
