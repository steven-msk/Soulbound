namespace SoulboundEngine.Client.Assets {
	public sealed record AssetKey(string address) {
		public override string ToString() => this.address;
	}
}
