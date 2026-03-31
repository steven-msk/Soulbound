namespace SoulboundEngine.Core.Render.Animation {
	using System;
	using Sprite = UnityEngine.Sprite;

	public readonly struct SpriteAnimation : IAnimationDefinition<Sprite> {
		private readonly AnimationKey animationKey;
		public readonly Sprite[] frameArray;
		public readonly float framerate;
		public readonly bool loop;

		public SpriteAnimation(AnimationKey animationKey, Sprite[] frameArray, float framerate, bool loop) {
			this.framerate = framerate;
			this.animationKey = animationKey;
			this.frameArray = frameArray;
			this.loop = loop;
		}

		public AnimationKey GetKey() => animationKey;


		// TODO: create centralized animation registry
		static SpriteAnimation() {
			Registry<SpriteAnimation>.SetContract(new RegistrationContract());
		}

		private sealed class RegistrationContract : IRegistrationContract<SpriteAnimation, IRegistrationKey<SpriteAnimation>> {
			public IRegistrationKey<SpriteAnimation> ValueToKey(SpriteAnimation value) {
				return new RegistrationKey(value.GetKey());
			}
		}

		public readonly struct RegistrationKey : IRegistrationKey<SpriteAnimation> {
			public readonly AnimationKey key;

			public RegistrationKey(AnimationKey key) {
				this.key = key;
			}

			public override bool Equals(object obj) {
				return obj is RegistrationKey other
					&& this.key.Equals(other.key);
			}

			public override int GetHashCode() => HashCode.Combine(key);
		}
	}
}
