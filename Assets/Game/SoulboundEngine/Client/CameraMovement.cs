using SoulboundEngine.Common.Math;
using SoulboundEngine.World.Player;
using System;
using UnityEngine;

namespace SoulboundEngine.Client {
	[Obsolete]
	public class CameraMovement : MonoBehaviour {
		void Update() {
			if (SoulboundClient.Instance.GetActiveWorldSession() is { } session) {
				PlayerEntity player = session.level.GetPlayer();
				Vec2d playerPos = player.GetPosition();
				Vector3 pos = new((float)playerPos.x, (float)playerPos.y);
				this.gameObject.transform.position = pos;
			}
		}
	}
}
