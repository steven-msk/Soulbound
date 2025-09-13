using SoulboundBackend.Core;
using UnityEngine;

namespace SoulboundBackend.Client {
	public class CameraMovement : MonoBehaviour {
		void Update() {
			Vector3 playerPos = GameManager.instance.Player.position;
			gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, transform.position.z);
		}
	}
}
