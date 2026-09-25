namespace SoulboundEngine.Serialization {
	using Newtonsoft.Json.Linq;

	public static class GlobalSaveVersion {
		// bumped when any change happens to any serialization source, including individual codecs
		private const int CURRENT = 0;
		private const int SAVE_VERSION_VERSION = 0;
		public static readonly Codec<int> VERSION_CODEC = Codecs.INT;

		public static int GetSinceVersion(JObject json) {
			return VERSION_CODEC.Decode(json["sinceVersion"] ?? JValue.CreateNull(), SAVE_VERSION_VERSION).ResultOrPartial().OrElse(Initial);
		}

		public static void WriteVersion(JObject obj, int version) {
			obj["sinceVersion"] = VERSION_CODEC.Encode(version);
		}

		public static int Current => CURRENT;

		public static int Initial => 0;
	}
}
