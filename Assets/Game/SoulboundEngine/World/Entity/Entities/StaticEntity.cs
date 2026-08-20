using SoulboundEngine.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.World.Entity {
	[PROTOTYPICAL]
	public sealed class StaticEntity : Entity {
		public StaticEntity(EntityDescriptor<StaticEntity> descriptor, Level.Level level)
			: base(descriptor, level) {
		}
	}
}
