using System;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulboundBackend.Client.Stats {
	public static class StatDisplayFormatter {

		public static Func<TValue, string, string> PlainNameFormat<TValue>(string hexColor) {
			if (UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out var color)) {
				return PlainNameFormat<TValue>(color);
			} else {
				UnityEngine.Debug.LogError($"Invalid hex color string {hexColor}. Are you sure the string starts with # ?");
				return PlainNameFormat<TValue>();
			}
		}

		public static Func<TValue, string, string> PlainNameFormat<TValue>(Color colorCode) {
			return (value, baseName) => $"<color=#{colorCode.ToHexString()}>{baseName}</color>";
		}

		public static Func<TValue, string, string> PlainNameFormat<TValue>() => PlainNameFormat<TValue>(Color.white);

		public static Func<TValue, string, string> PluralAdaptedNameFormat<TValue>(string hexColor) {
			if (UnityEngine.ColorUtility.TryParseHtmlString(hexColor, out var color)) {
				return PluralAdaptedNameFormat<TValue>(color);
			} else {
				UnityEngine.Debug.LogError($"Invalid hex color string {hexColor}. Are you sure the string starts with # ?");
				return PluralAdaptedNameFormat<TValue>();
			}
		}

		public static Func<TValue, string, string> PluralAdaptedNameFormat<TValue>(Color colorCode) {
			return (value, baseName) => {
				string adaptedName = Convert.ToDouble(value) > 1 ? $"{baseName}s" : baseName;
				return PlainNameFormat<TValue>(colorCode).Invoke(value, adaptedName);
			};
		}

		public static Func<TValue, string, string> PluralAdaptedNameFormat<TValue>() => PluralAdaptedNameFormat<TValue>(Color.white);

		public static Func<float, string> PercentageValueFormat() => value => $"{value * 100}%";

		public static Func<TValue, string> PlainValueFormat<TValue>() => value => value.ToString();

		public static Func<TValue, string> ColorPositiveNegative<TValue>() => value => Convert.ToDouble(value) > 0 ? "green" : "red";
	}
}