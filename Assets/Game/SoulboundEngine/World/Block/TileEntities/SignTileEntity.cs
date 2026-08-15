using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.UI.Screen;
using SoulboundEngine.Client.World.Block.State;
using SoulboundEngine.Client.World.Widget;

#nullable enable

namespace SoulboundEngine.Client.World.Block.Entity {
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

		public override void Read(JToken json) {
			this.text = (string)json["text"]!;
		}

		public override void Write(JObject json) {
			json["text"] = this.text;
		}

		public string GetText() => this.text;
		public void SetText(string text) => this.text = text;
	}
}
