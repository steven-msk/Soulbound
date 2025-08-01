using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlockState {
    // subject to change in the future
    // for now, it is just a reference to the block type
    // this is a placeholder for future properties like metadata, state, etc.

    public Block block { get; private set; }

    public BlockState(Block block) {
        this.block = block ?? throw new ArgumentNullException(nameof(block));
    }
}
