namespace SoulboundEngine.Client.Render.Item {
	using SoulboundEngine.Client.ItemSystem;
	using SoulboundEngine.Client.ItemSystem.Render;
	using System;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public abstract class ItemRenderer {
		public const float IMAGE_SIZE = 32f;
		public const float STACK_TEXT_SIZE = 8f;

		public delegate ItemRenderer Factory();

		internal abstract object CreateRenderStateBoxed(ItemStack stack, ItemModel model, ItemRenderContext context);
		internal abstract IItemView CreateViewBoxed(object state, ItemRenderContext context);
		internal abstract void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context);
		public abstract void DestroyView(IItemView view);

		public sealed class Default : ItemRenderer<ItemRenderState> {
			public override ItemRenderState CreateRenderState(ItemStack stack, ItemModel model, ItemRenderContext context) {
				return new ItemRenderState {
					showStackCount = context is ItemRenderContext.GUI && stack.item.IsStackable(),
					stack = stack,
					model = model,
				};
			}

			public override IItemView CreateView(ItemRenderState state, ItemRenderContext context) {
				switch (context) {
					case ItemRenderContext.GUI gui: {
							GameObject obj = new("UI Item View", typeof(UIItemView));
							obj.SetActive(false);
							obj.transform.SetParent(gui.parent, false);

							RectTransform rect = obj.GetComponent<RectTransform>();
							rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
							rect.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
							rect.anchoredPosition = Vector2.zero;

							Image itemImage = obj.AddComponent<Image>();
							itemImage.raycastTarget = false;
							TextMeshProUGUI stackText = this.CreateStackText(obj.GetComponent<RectTransform>());

							stackText.text = state.stack.quantity.ToString();
							stackText.enabled = state.showStackCount;

							UIItemView view = obj.GetComponent<UIItemView>();
							view.Init(stackText, itemImage);

							ItemModel model = state.model;
							Sprite sprite = model.GetSprite();
							view.GetItemImage().sprite = sprite;

							view.gameObject.SetActive(true);
							return IItemView.Of(obj);
						}
					default: throw new NotImplementedException();
				}
			}

			public override void DestroyView(IItemView view) => view.Destroy();

			public override void UpdateView(ItemRenderState state, IItemView view, ItemRenderContext context) {
			}

			private TextMeshProUGUI CreateStackText(RectTransform viewParent) {
				GameObject obj = new("Stack Text", typeof(RectTransform));
				obj.transform.SetParent(viewParent, false);

				TextMeshProUGUI text = obj.AddComponent<TextMeshProUGUI>();
				text!.autoSizeTextContainer = true;
				text.color = Color.white;
				text.fontSize = STACK_TEXT_SIZE;

				ContentSizeFitter sizeFitter = obj.AddComponent<ContentSizeFitter>();
				sizeFitter.verticalFit = sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

				RectTransform rect = obj.GetComponent<RectTransform>();
				rect.pivot = new Vector2(1f, 0f);
				rect.anchorMin = rect.anchorMax = new Vector2(0.9375f, 0.0625f);
				rect.anchoredPosition = Vector2.zero;

				return text;
			}
		}
	}

	public abstract class ItemRenderer<S> : ItemRenderer where S : ItemRenderState {
		public abstract S CreateRenderState(ItemStack stack, ItemModel model, ItemRenderContext context);

		public abstract IItemView CreateView(S state, ItemRenderContext context);
		public abstract void UpdateView(S state, IItemView view, ItemRenderContext context);

		internal override object CreateRenderStateBoxed(ItemStack stack, ItemModel model, ItemRenderContext context) {
			return this.CreateRenderState(stack, model, context);
		}
		internal override IItemView CreateViewBoxed(object state, ItemRenderContext context) {
			return this.CreateView((S)state, context);
		}
		internal override void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context) {
			this.UpdateView((S)state, view, context);
		}

	}
}
