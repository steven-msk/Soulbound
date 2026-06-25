using SoulboundEngine.Common;

#nullable enable

namespace SoulboundEngine.Client.World.Block {
	using Player = Player.Player;

	public abstract record BreakSource {
        public abstract bool fromPlayer { get; }
    }

    public record PlayerToolBreakSource(Player player) : BreakSource {
        public override bool fromPlayer => true;
    }

    [PROTOTYPICAL]
    public record TreeCollapseBreakSource(BreakSource origin) : BreakSource {
        public override bool fromPlayer => this.origin.fromPlayer;
    }
}
