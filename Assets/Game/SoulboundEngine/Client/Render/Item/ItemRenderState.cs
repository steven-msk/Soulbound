using SoulboundEngine.Client.ItemSystem;
using UnityEngine;

namespace SoulboundEngine.Client.Render.Items {
	public class ItemRenderState<I> where I : Item {
		public I item;
		public ItemStack stack;
		public Transform parent;
		public ItemModel model;
		public bool showStackText;
	}
}
