using PrimeTween;
using UnityEngine;

namespace SoulboundEngine.Core.Render.Animation {
	using Sprite = UnityEngine.Sprite;

	public sealed class SpriteAnimationPlayer {
		private readonly IAnimationTarget<Sprite> spriteTarget;
		private Tween? tween;

		public SpriteAnimationPlayer(IAnimationTarget<Sprite> spriteTarget) {
			this.spriteTarget = spriteTarget;
		}

		public void Play(SpriteAnimation animation) {
			Stop();

			float interval = 1f / animation.framerate;
			tween = Tween.Custom(
				startValue: 0,
				endValue: animation.frameArray.Length,
				duration: animation.frameArray.Length * interval,
				onValueChange: t => {
					int index = (int)t;
					index = Mathf.Clamp(index, 0, animation.frameArray.Length - 1);

					Sprite sprite = animation.frameArray[index];
					spriteTarget.Set(sprite);
				},
				cycles: animation.loop ? int.MaxValue : 1
			);
		}

		public void Stop() {
			tween?.Stop();
			tween = null;
		}
	}
}
