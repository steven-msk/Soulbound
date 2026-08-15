using SoulboundEngine.Common;
using SoulboundEngine.World.Player;

#nullable enable

namespace SoulboundEngine.World.Block {
	public abstract record BreakSource {
        public abstract bool fromPlayer { get; }
    }

    public record PlayerToolBreakSource(PlayerEntity player) : BreakSource {
        public override bool fromPlayer => true;
    }

    [PROTOTYPICAL]
    public record TreeCollapseBreakSource(BreakSource origin) : BreakSource {
        public override bool fromPlayer => this.origin.fromPlayer;
    }
}
