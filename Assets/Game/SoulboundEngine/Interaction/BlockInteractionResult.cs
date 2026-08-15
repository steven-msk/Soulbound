using SoulboundEngine.Item;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;
using SoulboundEngine.World.Block.State;
using SoulboundEngine.Client.World.Level;

#nullable enable

namespace SoulboundEngine.Interaction {
	public sealed record BlockInteractionResult(Level level, BlockPos blockPos, BlockState blockState, ItemStack stack, PlayerEntity? player);
}
