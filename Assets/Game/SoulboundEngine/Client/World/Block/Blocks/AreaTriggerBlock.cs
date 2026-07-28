using SoulboundEngine.Client.World.Block.Entity;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Common;
using SoulboundEngine.Core.States;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.World.Block {
	using PlayerEntity = Player.PlayerEntity;

	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block, ITileEntityProvider {
		public static readonly Property<bool> inArea = BoolProperty.Of("inArea");

		public AreaTriggerBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(inArea, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(inArea);
		}

		private void OnAreaEnter(Level.Level level, BlockPos selfPos, PlayerEntity player) {
			Logger.LogInfo("onAreaEnter");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, true));
		}

		private void OnAreaExit(Level.Level level, BlockPos selfPos, PlayerEntity player) {
			Logger.LogInfo("onAreaExit");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, false));
		}

		public TileEntity CreateTileEntity(BlockPos pos, BlockState state) {
			ObjectTileEntity tileEntity = ObjectTileEntity.Create(pos, state);

			tileEntity.onTriggerEnter += player => this.OnAreaEnter(tileEntity.GetLevel(), tileEntity.GetBlockPos(), player);
			tileEntity.onTriggerExit += player => this.OnAreaExit(tileEntity.GetLevel(), tileEntity.GetBlockPos(), player);

			return tileEntity;
		}
	}
}
