using SoulboundBackend.Client.UI.Screens;
using SoulboundBackend.Client.UI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Client.UI {
	public interface IItemContainerScreenScope : IScreenObject, IItemContainerScope {
		void AddItemContainer(UIItemContainerNode node);
		void RemoveItemContainer(UIItemContainerNode node);

		void SetTransitStack(UITransitStackNode node);
		void RemoveTransitStack();
	}
}
