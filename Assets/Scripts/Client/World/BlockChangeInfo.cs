using SoulboundBackend.Client.World;
using SoulboundBackend.Client.World.BlockSystem;

public readonly struct BlockChangeInfo {
    public readonly BlockPos blockPos;
    public readonly BlockState oldState;
    public readonly BlockState newState;
    public readonly Level level;
    public readonly BlockEventType eventType;
    public readonly Optional<BreakSource> breakSource;

    public BlockChangeInfo(
            BlockPos pos,
            BlockState oldState, 
            BlockState newState, 
            Level level, 
            BlockEventType eventType, 
            Optional<BreakSource> breakSource
        ) {
        this.blockPos = pos;
        this.oldState = oldState;
        this.newState = newState;
        this.level = level;
        this.eventType = eventType;
        this.breakSource = breakSource;
    }

    public override string ToString() {
        return $"BlockChangedEvent at {blockPos}: {oldState} -> {newState}";
    }
}