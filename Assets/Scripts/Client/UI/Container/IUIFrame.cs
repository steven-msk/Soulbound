using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public interface IUIFrame {
		void Apply(RectTransform rect, RectTransform parent);
	}
}
