using SoulboundEngine.Core;
using UnityEngine;

namespace SoulboundEngine.Client {
	public class CameraMovement : MonoBehaviour {
		void Update() {
			if (Soulbound.instance?.GetPlayerInstance() == null) return;
			Vector3 playerPos = Soulbound.instance.GetPlayerInstance().GetPos();
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
