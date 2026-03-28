using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.View {
	public class WorldItemDisplayView : MonoBehaviour {
		private ItemStack? itemStack;
		private SpriteRenderer itemRenderer = null!;

		public void Init(SpriteRenderer itemRenderer) {
			this.itemRenderer = itemRenderer;
		}

		public void SetStack(ItemStack? itemStack) {
			this.itemStack = itemStack;
			UpdateImage(itemStack);
		}

		private void UpdateImage(ItemStack? newStack) {

			// TODO: rework visual render approach for world item displays
			if (newStack != null) {
				//Sprite sprite = AssetManager.Resolve<Sprite>(newStack.item.aspect.icon.spriteKey);
				//itemRenderer.sprite = sprite;
			}
		}

		public ItemStack? GetStack() => itemStack;

		public void Destroy() => Destroy(gameObject);
	}
}
