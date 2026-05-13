using System;
using UnityEngine;

namespace SoulboundEngine.Client.UI.Screen {
	public interface IScreenRoot {
		[Obsolete]
		void AttachScreenObject(GameObject screenObject);
	}
}
