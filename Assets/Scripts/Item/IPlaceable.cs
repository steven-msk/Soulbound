public interface IPlaceable : IItemCapability {
	BlockState Place(ItemStack itemStack, BlockPos position);
}
