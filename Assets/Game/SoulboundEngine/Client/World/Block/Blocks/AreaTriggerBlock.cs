using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Block.TileEntity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Common;
using SoulboundEngine.Core.States;
using Logger = SoulboundEngine.Client.Debug.Logging.Logger;

namespace SoulboundEngine.Client.World.Block {
	using Player = Player.Player;

	[PROTOTYPICAL]
	public sealed class AreaTriggerBlock : Block {
		public static readonly Property<bool> inArea = BoolProperty.Of("inArea");

		public AreaTriggerBlock(Settings settings) 
			: base(settings) {
			this.SetDefaultState(this.DefaultState.With(inArea, false));
		}

		protected override void AppendProperties(StateManager<Block, BlockState>.Builder builder) {
			builder.Add(inArea);
		}

		public override bool HasTileEntity(Level.Level level, BlockPos blockPos, BlockState blockState) {
			return true;
		}

		public override TileEntity.TileEntity GetTileEntity(Level.Level level, BlockPos blockPos) {
			ObjectTileEntity tileEntity = new(TileEntityTypes.OBJECT, level, blockPos);

			tileEntity.onTriggerEnter += player => this.OnAreaEnter(level, blockPos, player);
			tileEntity.onTriggerExit += player => this.OnAreaExit(level, blockPos, player);

			return tileEntity;
		}

		private void OnAreaEnter(Level.Level level, BlockPos selfPos, Player player) {
			Logger.LogInfo("onAreaEnter");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, true));
		}

		private void OnAreaExit(Level.Level level, BlockPos selfPos, Player player) {
			Logger.LogInfo("onAreaExit");
			level.SetBlockState(selfPos, this.DefaultState.With(inArea, false));
		}
	}
}
