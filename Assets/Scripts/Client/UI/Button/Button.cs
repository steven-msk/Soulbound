using SoulboundBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI {
	public partial class GUI {
		public static class Button {
			[PROTOTYPICAL]
			public static ButtonBuilder New(GameObject prefab) {
				return ButtonBuilder.FromPrefab(prefab);
			}
		}
	}
}
