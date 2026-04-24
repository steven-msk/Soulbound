using Cysharp.Threading.Tasks;
using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Render.Animation;
using SoulboundEngine.Core.Render.Sprite;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class StackItem : Item {
		public override string name => $"Stack Item: {fullStackSize}";
		public override int fullStackSize { get; }

		[Obsolete]
		public StackItem(int fullStackSize) {
			this.fullStackSize = fullStackSize;

			// this will not pass to alpha prod
			// TODO: create proper animation registry

			AtlasSpriteResolver spriteResolver = new();
			AssetKey atlas = new("Items");
			
			Sprite[] frames = {
				spriteResolver.GetSprite(new SpriteRef(atlas, "idkwhatthisis")),
				spriteResolver.GetSprite(new SpriteRef(atlas, "bluething")),
				spriteResolver.GetSprite(new SpriteRef(atlas, "debugPointer"))
			};

			UniTask.Post(async () => {
				await UniTask.WaitForEndOfFrame();

				SpriteAnimation.Registry.Add(Items.GetIdentifier(this), new SpriteAnimation(
					new AnimationKey($"stack_item_animation_{fullStackSize}"),
					frames,
					fullStackSize,
					true
				));
			});

		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("idkwhatthisis", itemStack, Items.GetIdentifier(this));
		}

	}
}
