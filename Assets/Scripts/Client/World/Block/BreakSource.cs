using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
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
