namespace SoulboundEngine.UnityClient.Util {
	public static class ColorExtensions {
		public static UnityEngine.Color ToUnityColor(this Common.Color color) {
			return new UnityEngine.Color(color.r, color.g, color.b, color.a);
		}

		public static Common.Color ToEngineColor(this UnityEngine.Color color) {
			return new Common.Color(color.r, color.g, color.b, color.a);
		}
	}
}
