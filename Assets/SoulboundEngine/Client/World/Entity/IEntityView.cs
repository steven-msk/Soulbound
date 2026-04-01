using SoulboundEngine.Core.Registry;
using System;
using UnityEngine;

namespace SoulboundEngine.Client.World.EntitySystem {
	public interface IEntityView {
		Guid GetGuid();
		Identifier GetIdentifier();
		Vector2 GetPos();
	}
}
