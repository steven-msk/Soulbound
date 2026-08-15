using SoulboundEngine.Item;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Level;

#nullable enable

namespace SoulboundEngine.Client.Interaction {
	public sealed record BlockInteractionResult(Level level, BlockPos blockPos, BlockState blockState, ItemStack stack, PlayerEntity? player);
}
