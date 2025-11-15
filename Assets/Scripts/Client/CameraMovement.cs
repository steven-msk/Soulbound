using SoulboundBackend.Core;
using UnityEngine;

namespace SoulboundBackend.Client {
	public class CameraMovement : MonoBehaviour {
        void Update() {
            if (Soulbound.instance?.GetPlayerInstance() == null) {
                return;
            }
            Vector3 playerPos = Soulbound.instance.GetPlayerInstance().position;
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
