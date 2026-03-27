using SoulboundEngine.Client.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundEngine.Client.ItemSystem.Container.View {
	public interface ITransitStackHandle {
		void Destroy();
		void SetDisplayPosition(Vector2 position);
		void SetDisplayParent(RectTransform parent);
	}
}
