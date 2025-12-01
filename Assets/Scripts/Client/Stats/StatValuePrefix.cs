using SoulboundBackend.Common.Logging;
using System;

namespace SoulboundBackend.Client.Stats {
	[Obsolete]
	public class StatValuePrefix<TValue> {
		private static readonly Logger logger = Logger.CreateInstance();
		public static StatValuePrefix<TValue> None = new(_ => "");
		public static StatValuePrefix<TValue> Add;
		public static StatValuePrefix<TValue> AddAndSubtract;
		public static StatValuePrefix<TValue> Subtract = None;

		private readonly Func<TValue, string> prefixSupplier;

		private StatValuePrefix(Func<TValue, string> prefixSupplier) {
			this.prefixSupplier = prefixSupplier;
		}

		public string GetPrefix(TValue value) => prefixSupplier.Invoke(value);

		static StatValuePrefix() {
			if (typeof(TValue) == typeof(int)) {
				Add = new StatValuePrefix<TValue>(value => Convert.ToInt32(value) > 0 ? "+" : "");
				AddAndSubtract = Add;
			} else if (typeof(TValue) == typeof(float)) {
				Add = new StatValuePrefix<TValue>(value => Convert.ToSingle(value) > 0f ? "+" : "");
				AddAndSubtract = Add;
			} else {
				logger.LogWarning("Unexpected BonusAdmission type {}", typeof(TValue));
				Add = None;
				AddAndSubtract = None;
			}
		}
	}

}