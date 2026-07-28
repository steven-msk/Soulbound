using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;

namespace SoulboundEngine.Client.World.Entity {
	[PROTOTYPICAL]
	public sealed class StaticEntity : Entity {
		public StaticEntity(EntityDescriptor<StaticEntity> descriptor, Level.Level level)
			: base(descriptor, level) {
		}
	}
}
