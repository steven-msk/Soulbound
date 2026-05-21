using SoulboundEngine.Client.ItemSystem.Container;
using System;
using UnityEngine.UIElements;

namespace SoulboundEngine.Client.Render.Item {
	public class InteractableHotbarSlotDisplay : HotbarSlotDisplay, IInteractableUIToolkitSlotDisplay {
		public event Action<PointerDownEvent> onPointerDown;
		public event Action<PointerEnterEvent> onPointerEnter;
		public event Action<PointerLeaveEvent> onPointerLeave;
		public event Action<PointerUpEvent> onPointerUp;

		public InteractableHotbarSlotDisplay(IItemSlot slot, ItemRenderManager itemRenderManager) 
			: base(slot, itemRenderManager) {
		}

		public override void OnBind(VisualElement root) {
			base.OnBind(root);

			root.RegisterCallback<PointerDownEvent>(this.OnPointerDown);
			root.RegisterCallback<PointerUpEvent>(this.OnPointerUp);
			root.RegisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			root.RegisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		public new void Dispose() {
			base.Dispose();

			this.root.UnregisterCallback<PointerDownEvent>(this.OnPointerDown);
			this.root.UnregisterCallback<PointerUpEvent>(this.OnPointerUp);
			this.root.UnregisterCallback<PointerEnterEvent>(this.OnPointerEnter);
			this.root.UnregisterCallback<PointerLeaveEvent>(this.OnPointerLeave);
		}

		private void OnPointerDown(PointerDownEvent evt) => onPointerDown?.Invoke(evt);
		private void OnPointerUp(PointerUpEvent evt) => onPointerUp?.Invoke(evt);
		private void OnPointerEnter(PointerEnterEvent evt) => onPointerEnter?.Invoke(evt);
		private void OnPointerLeave(PointerLeaveEvent evt) => onPointerLeave?.Invoke(evt);
	}
}
