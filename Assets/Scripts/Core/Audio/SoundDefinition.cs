using System;
using UnityEngine;

namespace SoulboundBackend.Core.Audio {
	[CreateAssetMenu(menuName = "Audio/Sound", fileName = "sound")]
	public class SoundDefinition : ScriptableObject {
		[SerializeField] private AudioClip clip;
		[SerializeField, Range(0f, 1f)] private float volume = 1f;

		public AudioClip GetClip() => clip;
		public float GetVolume() => volume;
	}
}
