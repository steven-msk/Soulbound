using SoulboundEngine.Client.Player;
using SoulboundEngine.World.Level;
using SoulboundEngine.Item;
using SoulboundEngine.World.Block;
using SoulboundEngine.World.Block.State;

#nullable enable

namespace SoulboundEngine.Interaction {
	public sealed record BlockInteractionResult(Level level, BlockPos blockPos, BlockState blockState, ItemStack stack, PlayerEntity? player);
}
