using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public readonly struct BlockChangeInfo {
    public readonly BlockPos pos;
    public readonly BlockState oldState;
    public readonly BlockState newState;
    public readonly Level level;
    public readonly BlockEventType eventType;

    public BlockChangeInfo(BlockPos pos, BlockState oldState, BlockState newState, Level level, BlockEventType eventType) {
        this.pos = pos;
        this.oldState = oldState;
        this.newState = newState;
        this.level = level;
        this.eventType = eventType;
    }

    public override string ToString() {
        return $"BlockChangedEvent at {pos}: {oldState} -> {newState}";
    }
}