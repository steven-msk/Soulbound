using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Common;
using SoulboundEngine.Core;
using SoulboundEngine.Core.Assets;
using SoulboundEngine.Core.Render.Animation;
using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem {
	[PROTOTYPICAL]
	public sealed class StackItem : Item {
		public override string name => $"Stack Item: {fullStackSize}";

		public override int fullStackSize { get; }

		public StackItem(int fullStackSize)
			: base($"stackItem_{fullStackSize}") {
			this.fullStackSize = fullStackSize;


			// TODO: create centralized animation registry

			AtlasSpriteResolver spriteResolver = new();
			AssetKey atlas = new("Items");
			
			Sprite[] frames = {
				spriteResolver.GetSprite(new SpriteRef(atlas, "idkwhatthisis")),
				spriteResolver.GetSprite(new SpriteRef(atlas, "bluething")),
				spriteResolver.GetSprite(new SpriteRef(atlas, "debugPointer"))
			};

			Registry<SpriteAnimation>.Add(new SpriteAnimation(
				new AnimationKey($"stackItemAnimation_{fullStackSize}"),
				frames,
				fullStackSize,
				true
			));
		}

		public override ItemRenderData GetRenderData(ItemStack itemStack) {
			return new ItemRenderData("idkwhatthisis", itemStack, new AnimationKey($"stackItemAnimation_{fullStackSize}"));
		}

	}
}
