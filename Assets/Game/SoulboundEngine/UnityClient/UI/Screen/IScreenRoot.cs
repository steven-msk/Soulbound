using System;
using UnityEngine;

namespace SoulboundEngine.UnityClient.UI.Screen {
	public interface IScreenRoot {
		[Obsolete]
		void AttachScreenObject(GameObject screenObject);
	}
}
