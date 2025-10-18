using SoulboundBackend.Core;
using SoulboundBackend.Core.Bootstrap;
using UnityEngine;

namespace SoulboundBackend.Client {
	public class CameraMovement : MonoBehaviour {
        void Update() {
            if (Soulbound.instance?.GetActiveLevel()?.Player == null) {
                return;
            }
            Vector3 playerPos = Soulbound.instance.GetActiveLevel().Player.position;
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
