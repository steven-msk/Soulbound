namespace SoulboundEngine.UnityClient.Assets {
	public sealed record AssetKey(string address) {
		public override string ToString() => this.address;
	}
}
