using SoulboundEngine.Common.Math;
using SoulboundEngine.Registry;
using System;

namespace SoulboundEngine.World.Entity {
	public interface IEntityView {
		Guid GetGuid();
		Identifier GetIdentifier();
		Vec2d GetPos();
	}
}
