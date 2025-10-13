using SoulboundBackend.Client.ItemSystem;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
    public abstract record BreakSource {
        public abstract bool fromPlayer { get; }
    }

    public record PlayerToolBreakSource(PlayerController player, IBreakingTool? tool) : BreakSource {
        public override bool fromPlayer => true;
    }
}