using SoulboundEngine.Client.Players;
using SoulboundEngine.Common;

#nullable enable

namespace SoulboundEngine.Client.World.BlockSystem {
    public abstract record BreakSource {
        public abstract bool fromPlayer { get; }
    }

    public record PlayerToolBreakSource(Player player) : BreakSource {
        public override bool fromPlayer => true;
    }

    [PROTOTYPICAL]
    public record TreeCollapseBreakSource(BreakSource origin) : BreakSource {
        public override bool fromPlayer => origin.fromPlayer;
    }
}
