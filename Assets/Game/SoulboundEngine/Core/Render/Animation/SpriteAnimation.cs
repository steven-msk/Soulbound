namespace SoulboundEngine.Core.Render.Animation {
	using SoulboundEngine.Core.Registry;
	using System;
	using System.Collections.Generic;
	using Sprite = UnityEngine.Sprite;

	[Obsolete]
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

		public AnimationKey GetKey() => this.animationKey;

		[Obsolete("Will be removed before alpha prod")]
		public static class Registry {
			private static readonly Dictionary<Identifier, SpriteAnimation> animationsById = new();

			public static void Add(Identifier id, SpriteAnimation animation) {
				animationsById.Add(id, animation);
			}

			public static SpriteAnimation? Get(Identifier id) {
				return animationsById.GetValueOrDefault(id);
			}
		}
	}
}
