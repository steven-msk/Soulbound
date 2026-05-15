namespace SoulboundEngine.Client.Render.Item {
	using SoulboundEngine.Client.ItemSystem;
	using SoulboundEngine.Core;
	using System;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.UIElements;

	public abstract class ItemRenderer {
		public const float IMAGE_SIZE = 64f;
		public const float STACK_TEXT_SIZE = 13f;

		public delegate ItemRenderer Factory();

		internal abstract object CreateRenderStateBoxed(ItemStack stack, ItemRenderContext context);
		internal abstract IItemView CreateViewBoxed(object state, ItemModel model, ItemRenderContext context);
		internal abstract void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context);
		public abstract void DestroyView(IItemView view, ItemRenderContext context);

		public sealed class Default : ItemRenderer<ItemRenderState> {
			public override ItemRenderState CreateRenderState(ItemStack stack, ItemRenderContext context) {
				return new ItemRenderState {
					showStackCount = (context is ItemRenderContext.GUI || context is ItemRenderContext.UIToolkit)
						&& stack.item.IsStackable(),
					stack = stack
				};
			}

			public override IItemView CreateView(ItemRenderState state, ItemModel model, ItemRenderContext context) {
				switch (context) {
					case ItemRenderContext.GUI gui: {
							GameObject obj = new("UI Item", typeof(RectTransform));
							obj.SetActive(false);
							obj.transform.SetParent(gui.parent, false);

							RectTransform rect = obj.GetComponent<RectTransform>();
							rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
							rect.sizeDelta = new Vector2(IMAGE_SIZE, IMAGE_SIZE);
							rect.anchoredPosition = Vector2.zero;

							Sprite sprite = model.GetSprite();
							UnityEngine.UI.Image itemImage = obj.AddComponent<UnityEngine.UI.Image>();
							itemImage.sprite = sprite;
							itemImage.raycastTarget = false;

							TextMeshProUGUI stackText = this.CreateStackText(rect);
							stackText.text = state.stack.quantity.ToString();
							stackText.enabled = state.showStackCount;

							obj.SetActive(true);
							return IItemView.Of(obj);
						}
					case ItemRenderContext.UIToolkit uiToolkit: {
							VisualElement display = uiToolkit.GetItemDisplay();
							Label stackText = uiToolkit.GetStackCount();

							display.style.backgroundImage = new StyleBackground(model.GetSprite());
							stackText.text = state.stack.quantity.ToString();
							stackText.style.display = state.showStackCount ? DisplayStyle.Flex : DisplayStyle.None;

							uiToolkit.root.style.display = DisplayStyle.Flex;
							return IItemView.Of(uiToolkit.root);
						}
					case ItemRenderContext.World world: {
							GameObject obj = new("Item");
							obj.SetActive(false);
							obj.transform.position = world.position;
							obj.transform.localScale = model.GetScaleTo(ItemRenderers.TILE_SIZE);

							SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
							spriteRenderer.sprite = model.GetSprite();

							Rigidbody2D rigidbody = obj.AddComponent<Rigidbody2D>();
							rigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
							rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

							BoxCollider2D physicsCollider = obj.AddComponent<BoxCollider2D>();
							physicsCollider.excludeLayers = LayerMask.GetMask(Layers.EntityCharacter);

							BoxCollider2D pickupCollider = obj.AddComponent<BoxCollider2D>();
							pickupCollider.isTrigger = true;

							obj.SetActive(true);
							return IItemView.Of(obj);
						}
					default: throw new NotImplementedException();
				}
			}

			public override void DestroyView(IItemView view, ItemRenderContext context) {
				if (context is ItemRenderContext.UIToolkit uiToolkit) {
					VisualElement display = uiToolkit.GetItemDisplay();
					Label stackText = uiToolkit.GetStackCount();

					display.style.backgroundImage = new StyleBackground((Sprite)null);
					stackText.text = "";
					stackText.style.display = DisplayStyle.None;

					return;
				}

				view.Destroy();
			}

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
		public abstract S CreateRenderState(ItemStack stack, ItemRenderContext context);

		public abstract IItemView CreateView(S state, ItemModel model, ItemRenderContext context);
		public abstract void UpdateView(S state, IItemView view, ItemRenderContext context);

		internal override object CreateRenderStateBoxed(ItemStack stack, ItemRenderContext context) {
			return this.CreateRenderState(stack, context);
		}

		internal override IItemView CreateViewBoxed(object state, ItemModel model, ItemRenderContext context) {
			return this.CreateView((S)state, model, context);
		}

		internal override void UpdateViewBoxed(object state, IItemView view, ItemRenderContext context) {
			this.UpdateView((S)state, view, context);
		}
	}
}
