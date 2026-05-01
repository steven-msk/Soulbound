using SoulboundEngine.Client.ItemSystem;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Item {
	public interface IItemModelResolver {
		ItemModel Resolve(ItemStack itemStack);

		public sealed class Default : IItemModelResolver {
			private readonly Sprite sprite;

			public Default(Sprite sprite) {
				this.sprite = sprite;
			}

			public ItemModel Resolve(ItemStack itemStack) {
				return new BasicItemModel(this.sprite);
			}
		}
	}
}
