using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct BlockChangedEvent {
    public BlockPos pos;
    public BlockState oldState;
    public BlockState newState;

    public BlockChangedEvent(BlockPos pos, BlockState oldState, BlockState newState) {
        this.pos = pos;
        this.oldState = oldState;
        this.newState = newState;
    }

    public override string ToString() {
        return $"BlockChangedEvent at {pos}: {oldState} -> {newState}";
    }
}