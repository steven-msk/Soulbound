using SoulboundEngine.Client.Players;
using UnityEngine;

namespace SoulboundEngine.Client {
	public class CameraMovement : MonoBehaviour {
		void Update() {
			PlayerTransform player = FindFirstObjectByType<PlayerTransform>();
			if (player == null) return;

			Vector3 playerPos = player.GetPos();
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
