using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public sealed class ButtonFactory {
		[PROTOTYPICAL]
		public ButtonBuilder New(GameObject prefab) {
			return ButtonBuilder.FromPrefab(prefab);
		}
	}
}
