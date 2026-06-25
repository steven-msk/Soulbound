using SoulboundEngine.Common;

#nullable enable

namespace SoulboundEngine.Client.World.Block {
	using PlayerEntity = Player.PlayerEntity;

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
