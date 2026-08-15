using SoulboundEngine.Item;
using SoulboundEngine.Item.Container;
using SoulboundEngine.Client.UI;
using SoulboundEngine.Client.UI.UXMLBindings;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

#nullable enable

namespace SoulboundEngine.Client.Render.Item {
	public class UXMLItemSlotDisplay : UXMLWidget, IItemSlotDisplay {
		private static readonly Identifier ITEM_DISPLAY_ELEMENT = Identifier.Of("soulbound:slot/item_display");
		private static readonly Identifier STACK_COUNT_ELEMENT = Identifier.Of("soulbound:slot/stack_count");
		private static readonly Identifier DURABILITY_BAR_ELEMENT = Identifier.Of("soulbound:slot/durability_bar");
		protected readonly ItemRenderManager itemRenderManager;
		protected readonly ItemRenderHandle renderHandle;
		public event Action<PointerDownEvent>? onPointerDown;
		public event Action<PointerEnterEvent>? onPointerEnter;
		public event Action<PointerLeaveEvent>? onPointerLeave;
		public event Action<PointerUpEvent>? onPointerUp;
		private bool isHovering;
		private bool interactable;
		private bool isTooltipVisible;
		private bool showTooltip;
		private ProgressBar durabilityBar = null!;
		protected IItemView? view { get; private set; }
		protected ItemStack stack { get; private set; }
		protected IItemSlot? slot { get; private set; }

		public UXMLItemSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager, bool interactable, bool showTooltip = true) {
			this.interactable = interactable;
			this.showTooltip = showTooltip;
			this.slot = slot;
			this.itemRenderManager = itemRenderManager;
			this.renderHandle = new ItemRenderHandle(this);
		}

		protected UXMLItemSlotDisplay(ItemRenderManager itemRenderManager, bool interactable, bool showTooltip)
			: this(null!, itemRenderManager, interactable, showTooltip) {
		}

		protected ItemRenderContext RenderContext => new ItemRenderContext.UXML(this.root, this.GetItemDisplayId(), this.GetStackCountId());
		
		/// <summary> Used to bind the VisualElement to the current slot. </summary>
		public sealed override void OnBind(VisualElement root) {
			this.root = root;
			if (this.slot != null) this.stack = this.slot.GetStack();
			this.OnBind();
		}

		/// <summary> Used to bind the VisualElement to the given stack </summary>
		protected void OnBind(VisualElement root, ItemStack stack) {
			this.root = root;
			this.stack = stack;
			this.OnBind();
		}

		/// <summary> Used to bind the VisualElement to the given slot </summary>
		protected void OnBind(VisualElement root, IItemSlot slot) {
			this.root = root;
			this.stack = slot.GetStack();
			this.slot = slot;
			this.OnBind();
		}

		private void OnBind() {
			this.durabilityBar = this.root.Get<ProgressBar>(this.GetDurabilityBarId());
			if (this.interactable) this.RegisterPointerCallbacks();
			if (this.slot != null) this.slot.stackChanged += this.StackChanged;
			this.Prepare();
			this.Render();
		}

		/// <summary>
		/// Called just before <see cref="Render"/> is called
		/// </summary>
		protected virtual void Prepare() {
		}

		/// <summary> Must override if the item display element ID originates from a different UXML file </summary>
		protected virtual Identifier GetItemDisplayId() => ITEM_DISPLAY_ELEMENT;

		/// <summary> Must override if the stack count element ID originates from a different UXML file </summary>
		protected virtual Identifier GetStackCountId() => STACK_COUNT_ELEMENT;

		/// <summary> Must override if the durability bar element ID originates from a different UXML file </summary>
		protected virtual Identifier GetDurabilityBarId() => DURABILITY_BAR_ELEMENT;

		private void StackChanged(ItemStack oldStack, ItemStack newStack) => this.SetStack(newStack);

		public void SetStack(ItemStack stack) {
			this.stack = stack;
			this.Render(stack);
		}

		protected void SetStackDontRender(ItemStack stack) {
			this.stack = stack;
		}

		protected void SetSlot(IItemSlot? slot) {
			if (this.slot != null) this.slot.stackChanged -= this.StackChanged;
			this.slot = slot;
			if (slot != null) slot.stackChanged += this.StackChanged;
		}

		/// <summary>
		/// Renders the current stack.
		/// <b>Do not call this unless this widget has been added to a screen.</b>
		/// </summary>
		protected void Render() => this.Render(this.stack);

		/// <summary>
		/// Renders the given stack.
		/// <b>Do not call this unless this widget has been added to a screen.</b>
		/// </summary>
		protected virtual void Render(ItemStack stack) {
			this.UpdateTooltip();
			this.UpdateDurability();

			if (stack.IsEmpty()) {
				this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
				this.view = null;
				return;
			}
			this.view = this.itemRenderManager.Render(this.renderHandle, stack, this.RenderContext);
		}

		public override void Dispose() {
			this.itemRenderManager.Destroy(this.renderHandle, this.RenderContext);
			if (this.slot != null) this.slot.stackChanged -= this.StackChanged;
			this.view = null;

			if (this.interactable) this.UnregisterPointerCallbacks();
			onPointerDown = null;
			onPointerEnter = null;
			onPointerLeave = null;
			onPointerUp = null;

			if (this.isTooltipVisible) this.Screen.ClearTooltip();
			this.ClearDurabilityBar();
		}

		public bool IsInteractable() => this.interactable;

		public void SetInteractable(bool interactable) {
			bool previouslyInteractable = this.interactable;
			this.interactable = interactable;
			if (interactable && !previouslyInteractable) {
				this.RegisterPointerCallbacks();
			} else if (!interactable) {
				this.UnregisterPointerCallbacks();
			}
		}

		public void ShowTooltip(bool showTooltip) {
			this.showTooltip = showTooltip;
		}

		public virtual void SetAsMainSlot() {
		}

		public virtual void UnsetMainSlot() {
		}

		private void OnPointerEnter(PointerEnterEvent evt) {
			onPointerEnter?.Invoke(evt);
			this.isHovering = true;
			this.UpdateTooltip();
		}

		private void OnPointerLeave(PointerLeaveEvent evt) {
			onPointerLeave?.Invoke(evt);
			this.isHovering = false;
			this.UpdateTooltip();
		}

		private void OnPointerDown(PointerDownEvent evt) => onPointerDown?.Invoke(evt);

		private void OnPointerUp(PointerUpEvent evt) => onPointerUp?.Invoke(evt);

		private void RegisterPointerCallbacks() {
			this.root.RegisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.RegisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.RegisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.RegisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		private void UnregisterPointerCallbacks() {
			this.root.UnregisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.UnregisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.UnregisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.UnregisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		protected void UpdateTooltip() {
			if (!this.showTooltip) return;

			if (this.isHovering && !this.stack.IsEmpty()) {
				this.Screen.SetTooltip(this.GetTooltip(this.stack));
				this.isTooltipVisible = true;
			} else {
				this.Screen.ClearTooltip();
			}
		}

		protected virtual string GetTooltip(ItemStack stack) {
			List<string> tooltips = new();
			stack.AppendTooltip(tooltips);
			return string.Join('\n', tooltips);
		}

		protected void UpdateDurability() {
			this.ClearDurabilityBar();

			this.durabilityBar.style.display = !this.stack.HasDurability()
				? DisplayStyle.None : DisplayStyle.Flex;
			if (this.durabilityBar.style.display.value == DisplayStyle.None) return;

			this.durabilityBar.highValue = this.stack.GetMaxDurability();
			this.durabilityBar.value = this.stack.GetCurrentDurability();
		}

		protected void ClearDurabilityBar() {
			this.durabilityBar.highValue = 0;
			this.durabilityBar.value = 0;
		}

	}
}
