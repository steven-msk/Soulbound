using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using UnityEngine;

namespace SoulboundBackend.Client {
	public class CameraMovement : MonoBehaviour {
        void Update() {
            if (LevelManager.instance?.Player == null) {
                return;
            }
            Vector3 playerPos = LevelManager.instance.Player.position;
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
