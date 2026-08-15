using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

namespace SoulboundEngine.World.Entity {
	public interface IEntityView {
		Guid GetGuid();
		Identifier GetIdentifier();
		Vector2 GetPos();
	}
}
