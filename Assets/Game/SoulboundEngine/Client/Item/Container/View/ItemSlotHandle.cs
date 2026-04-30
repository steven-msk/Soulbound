using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.Render.Items;
using SoulboundEngine.Client.UI.Tooltips;
using SoulboundEngine.Core.Render.Sprite;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	[RequireComponent(typeof(RectTransform))]
	public class ItemSlotHandle : MonoBehaviour, IItemSlotHandle, ITooltipTrigger {
		private IItemSlot slot = null!;
		private ITooltip tooltip = null!;
		private ITooltipRenderer tooltipRenderer = null!;
		private IItemSlotEventListener eventListener = null!;
		private ITooltipHandle? tooltipHandle;
		private RectTransform rect = null!;
		private UIItemView? itemView;
		private ItemStack? stack;

		private readonly UIItemRenderer itemRenderer = new(new AtlasSpriteResolver());
		private readonly ItemModelResolver modelResolver = new();

		public void Init(IItemSlot slot, IItemSlotEventListener eventListener) {
			this.slot = slot;
			this.eventListener = eventListener;
			this.rect = this.GetComponent<RectTransform>();
			slot.setStack += this.SetStack;
			this.SetStack(slot.GetStack());
		}

		private void SetStack(ItemStack? stack) {
			if (this.stack != null) this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
			this.OnStackQuantityChanged(this.stack?.quantity ?? 0, stack?.quantity ?? 0);
			this.stack = stack;

			if (stack != null) {
				this.DestroyTooltip();
				stack.onQuantityChanged += this.OnStackQuantityChanged;

				// prototypical
				this.SetTooltip(new ItemTooltip(stack.item));
			} else {
				this.SetTooltip(null!);
			}
			this.Render(stack);
		}

		private void Render(ItemStack? itemStack) {
			if (itemStack == null && this.itemView != null) {
				this.itemView.Destroy();
			} else if (itemStack != null) {
				if (this.itemView == null) this.itemView = this.itemRenderer.CreateView(this.rect);

				ItemRenderData renderData = itemStack.item.GetRenderData(itemStack);
				ItemRenderModel model = this.modelResolver.Resolve(renderData);
				this.itemRenderer.Render(this.itemView, model);
			}
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) => this.tooltipRenderer = tooltipRenderer;
		public void SetTooltip(ITooltip tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			this.eventListener.OnPointerEnter(this.slot.GetIndex(), eventData);
			if (this.tooltip != null) {
				this.tooltipHandle = this.tooltipRenderer.RenderTooltip(this.tooltip);
			}
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			this.eventListener.OnPointerExit(this.slot.GetIndex(), eventData);
			this.DestroyTooltip();
		}
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			this.eventListener.OnPointerDown(this.slot.GetIndex(), eventData);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			this.eventListener.OnPointerUp(this.slot.GetIndex(), eventData);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				this.DestroyTooltip();
				this.tooltip = null!;

				if (this.stack != null) {
					this.stack.onQuantityChanged -= this.OnStackQuantityChanged;
				}
				this.stack = null;
				if (this.itemView != null) this.itemView.Destroy();
			} else if (this.stack != null) this.Render(this.stack);
		}

		public void SetVisible(bool visible) {
			this.gameObject.SetActive(visible);
			if (!visible) this.DestroyTooltip();
		}

		private void DestroyTooltip() {
			this.tooltipHandle?.Destroy();
			this.tooltipHandle = null;
		}

		public void ToggleVisibility() => this.SetVisible(!this.gameObject.activeSelf);
	}
}
