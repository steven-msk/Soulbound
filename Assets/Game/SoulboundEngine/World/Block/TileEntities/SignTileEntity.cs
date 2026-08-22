namespace SoulboundEngine.World.Block.Entity {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Client.UI.Screen;
	using SoulboundEngine.World.Block.State;
	using SoulboundEngine.World.Widget;

#nullable enable

	public class SignTileEntity : TileEntity {
		private string text = "a sign";

		private SignTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState) 
			: base(tileEntityType, blockPos, blockState) {
		}

		public SignTileEntity(BlockPos blockPos, BlockState state)
			: this(TileEntityType.SIGN, blockPos, state) {
		}

		public static SignTileEntity Create(BlockPos blockPos, BlockState state) {
			return new SignTileEntity(blockPos, state);
		}

		public WorldWidgetHandle? widgetHandle { get; set; }
		public IScreenHandle? screenHandle { get; set; }

		public override void ReadAdditional(JObject json) {
			this.text = (string)json["text"]!;
		}

		public override void WriteAdditional(JObject json) {
			json["text"] = this.text;
		}

		public string GetText() => this.text;
		public void SetText(string text) => this.text = text;
	}
}
