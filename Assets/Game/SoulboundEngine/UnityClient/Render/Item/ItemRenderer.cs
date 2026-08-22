namespace SoulboundEngine.UnityClient.Render.Item {
	using SoulboundEngine.Item;
	using System;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.UIElements;

	public abstract class ItemRenderer {
		public const float IMAGE_SIZE = 64f;
		public const float STACK_TEXT_SIZE = 13f;

		public delegate ItemRenderer Factory();

		internal abstract object InternalCreateRenderState(ItemStack stack, ItemRenderContext context);

		internal abstract ItemViewHandle InternalCreate(object state, ItemModel model, ItemRenderContext context);

		internal abstract void InternalUpdate(object state, ItemViewHandle view, ItemRenderContext context);

		public abstract void Destroy(ItemViewHandle view, ItemRenderContext context);

		public sealed class Default : ItemRenderer<ItemRenderState> {
			public override ItemRenderState CreateRenderState(ItemStack stack, ItemRenderContext context) {
				return new ItemRenderState {
					showStackCount = (context is ItemRenderContext.UGUI or ItemRenderContext.UXML)
						&& stack.GetItem().IsStackable(),
					stack = stack
				};
			}

			public override ItemViewHandle Create(ItemRenderState state, ItemModel model, ItemRenderContext context) {
				switch (context) {
					case ItemRenderContext.UGUI gui: {
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
							stackText.text = state.stack.count.ToString();
							stackText.enabled = state.showStackCount;

							obj.SetActive(true);
							return ItemViewHandle.Of(obj);
						}
					case ItemRenderContext.UXML uxmlContext: {
							VisualElement display = uxmlContext.GetItemDisplay();
							Label stackText = uxmlContext.GetStackCount();

							display.style.backgroundImage = new StyleBackground(model.GetSprite());
							uxmlContext.SetVisible(display, true);

							stackText.text = state.stack.count.ToString();
							uxmlContext.SetVisible(stackText, state.showStackCount);

							display.pickingMode = PickingMode.Ignore;
							stackText.pickingMode = PickingMode.Ignore;

							uxmlContext.SetVisible(uxmlContext.root, true);
							return ItemViewHandle.Of(display);
						}
					case ItemRenderContext.World world: {
							GameObject obj = new("Item");
							obj.SetActive(false);
							obj.transform.position = new Vector3((float)world.position.x, (float)world.position.y);
							obj.transform.localScale = model.GetScaleToWorldSize(Vector2.one);

							Sprite sprite = model.GetSprite();
							SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
							spriteRenderer.sprite = sprite;

							obj.SetActive(true);
							return ItemViewHandle.Of(obj);
						}
					default: throw new NotImplementedException();
				}
			}

			public override void Destroy(ItemViewHandle view, ItemRenderContext context) {
				if (context is ItemRenderContext.UXML uxmlContext) {
					VisualElement display = uxmlContext.GetItemDisplay();
					display.style.backgroundImage = new StyleBackground((Sprite)null);
					uxmlContext.SetVisible(display, false);
					Label stackText = uxmlContext.GetStackCount();
					stackText.text = "";
					uxmlContext.SetVisible(stackText, false);
				} else if (context is ItemRenderContext.World or ItemRenderContext.UGUI) {
					ItemViewHandle.GameObjectBacked objView = (ItemViewHandle.GameObjectBacked)view;
					GameObject.Destroy(objView.gameObject);
				}
			}

			public override void Update(ItemRenderState state, ItemViewHandle view, ItemRenderContext context) {
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

		public abstract ItemViewHandle Create(S state, ItemModel model, ItemRenderContext context);

		public abstract void Update(S state, ItemViewHandle view, ItemRenderContext context);

		internal override object InternalCreateRenderState(ItemStack stack, ItemRenderContext context) {
			return stack.IsEmpty()
				? throw new InvalidOperationException("Creating an empty item render state is not allowed")
				: this.CreateRenderState(stack, context);
		}

		internal override ItemViewHandle InternalCreate(object state, ItemModel model, ItemRenderContext context) {
			return this.Create((S)state, model, context);
		}

		internal override void InternalUpdate(object state, ItemViewHandle view, ItemRenderContext context) {
			this.Update((S)state, view, context);
		}
	}
}
