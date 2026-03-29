using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.UI.Tooltips;
using SoulboundEngine.Core.Render;
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
			rect = GetComponent<RectTransform>();
			slot.setStack += SetStack;
			SetStack(slot.GetStack());
		}

		private void SetStack(ItemStack? stack) {
			if (this.stack != null) this.stack.onQuantityChanged -= OnStackQuantityChanged;
			OnStackQuantityChanged(this.stack?.quantity ?? 0, stack?.quantity ?? 0);
			this.stack = stack;

			if (stack != null) {
				DestroyTooltip();
				stack.onQuantityChanged += OnStackQuantityChanged;

				// prototypical
				SetTooltip(new ItemTooltip(stack.item));
			} else {
				SetTooltip(null!);
			}
			Render(stack);
		}

		private void Render(ItemStack? itemStack) {
			if (itemStack == null && itemView != null) {
				itemView.Destroy();
			} else if (itemStack != null) {
				if (itemView == null) itemView = itemRenderer.CreateView(rect);

				ItemRenderData renderData = itemStack.item.GetRenderData(itemStack);
				ItemRenderModel model = modelResolver.Resolve(renderData);
				itemRenderer.Render(itemView, model);
			}
		}

		void ITooltipTrigger.Init(ITooltipRenderer tooltipRenderer) => this.tooltipRenderer = tooltipRenderer;
		public void SetTooltip(ITooltip tooltip) => this.tooltip = tooltip;
		void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
			eventListener.OnPointerEnter(slot.GetIndex(), eventData);
			if (tooltip != null) {
				tooltipHandle = tooltipRenderer.RenderTooltip(tooltip);
			}
		}
		void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
			eventListener.OnPointerExit(slot.GetIndex(), eventData);
			DestroyTooltip();
		}
		void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
			eventListener.OnPointerDown(slot.GetIndex(), eventData);
		}
		void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
			eventListener.OnPointerUp(slot.GetIndex(), eventData);
		}

		private void OnStackQuantityChanged(int old, int @new) {
			if (@new <= 0) {
				DestroyTooltip();
				tooltip = null!;

				if (stack != null) {
					stack.onQuantityChanged -= OnStackQuantityChanged;
				}
				stack = null;
				if (itemView != null) itemView.Destroy();
			} else if (stack != null) Render(stack);
		}

		public void SetVisible(bool visible) {
			gameObject.SetActive(visible);
			if (!visible) DestroyTooltip();
		}

		private void DestroyTooltip() {
			tooltipHandle?.Destroy();
			tooltipHandle = null;
		}

		public void ToggleVisibility() => SetVisible(!gameObject.activeSelf);
	}
}
