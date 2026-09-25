namespace SoulboundEngine.World.Gen {
	using SoulboundEngine.Common.Math.Random;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Block.State;
	using System.Collections.Generic;
	using System.Linq;

	public interface IBlockStateTest {
		private static readonly IBlockStateTest ALWAYS_TRUE = new ConstantValue(true);
		private static readonly IBlockStateTest ALWAYS_FALSE = new ConstantValue(false);

		public static IBlockStateTest AlwaysTrue => ALWAYS_TRUE;

		public static IBlockStateTest AlwaysFalse => ALWAYS_FALSE;

		public static IBlockStateTest Constant(bool value) => new ConstantValue(value);

		public static IBlockStateTest MatchBlock(Block block) => new BlockMatchTest(new HashSet<Block> { block });

		public static IBlockStateTest MatchBlocks(params Block[] blocks) => new BlockMatchTest(blocks.ToHashSet());

		public static IBlockStateTest MatchState(BlockState blockState) => new BlockStateMatchTest(new HashSet<BlockState> { blockState });

		public static IBlockStateTest MatchStates(params Block[] blocks) => new BlockMatchTest(blocks.ToHashSet());

		public static IBlockStateTest RandomBlock(Block block, float probability) => new RandomBlockMatchTest(block, probability);

		public static IBlockStateTest RandomState(BlockState blockState, float probability) => new RandomBlockStateMatchTest(blockState, probability);

		bool Test(BlockState state, IRandom random);

		private record ConstantValue(bool value) : IBlockStateTest {
			public bool Test(BlockState state, IRandom random) => this.value;
		}

		private record BlockMatchTest(HashSet<Block> blocks) : IBlockStateTest {
			public bool Test(BlockState state, IRandom random) => this.blocks.Contains(state.block);
		}

		private record BlockStateMatchTest(HashSet<BlockState> states) : IBlockStateTest {
			public bool Test(BlockState state, IRandom random) => this.states.Contains(state);
		}

		private record RandomBlockMatchTest(Block block, float probability) : IBlockStateTest {
			public bool Test(BlockState state, IRandom random) {
				return state.IsOf(this.block) && random.NextFloat() < this.probability;
			}
		}

		private record RandomBlockStateMatchTest(BlockState blockState, float probability) : IBlockStateTest {
			public bool Test(BlockState state, IRandom random) {
				return state == this.blockState && random.NextFloat() < this.probability;
			}
		}
	}
}
