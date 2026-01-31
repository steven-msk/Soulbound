using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SoulboundBackend.Client.UI.Screens {
	public interface IScreenObjectFactory {
		GameObject CreateGameObject();
		IScreenObject CreateSceneObject(Screen screen, GameObject obj);
	}
}
