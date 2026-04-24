using SoulboundEngine.Client.World.LevelDomain;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.World.EntitySystem {
	[PROTOTYPICAL]
	public sealed class StaticEntity : Entity {
		public StaticEntity(EntityDescriptor<StaticEntity> descriptor, Level level)
			: base(descriptor, level) {
		}
	}
}
