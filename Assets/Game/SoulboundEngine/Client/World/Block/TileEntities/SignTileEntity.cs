using Newtonsoft.Json.Linq;
using SoulboundEngine.Client.World.Block.State;

namespace SoulboundEngine.Client.World.Block.Entity {
	public class SignTileEntity : TileEntity {
		private string text;

		private SignTileEntity(TileEntityType tileEntityType, BlockPos blockPos, BlockState blockState) 
			: base(tileEntityType, blockPos, blockState) {
		}

		public SignTileEntity(BlockPos blockPos, BlockState state)
			: this(TileEntityType.SIGN, blockPos, state) {
		}

		public static SignTileEntity Create(BlockPos blockPos, BlockState state) {
			return new SignTileEntity(blockPos, state);
		}

		public override void Read(JToken json) {
			this.text = (string)json["text"];
		}

		public override void Write(JObject json) {
			json["text"] = this.text;
		}

		public string GetText() => this.text;
	}
}
