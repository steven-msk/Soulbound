namespace SoulboundEngine.Core.Render.Animation {
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
	}
}
