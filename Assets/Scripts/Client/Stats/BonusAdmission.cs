using System;
using UnityEngine;

public class BonusAdmission<TValue> {
	public static BonusAdmission<TValue> None = new(_ => "");
	public static BonusAdmission<TValue> Add;
	public static BonusAdmission<TValue> AddAndSubtract;
	public static BonusAdmission<TValue> Subtract = None;

	private readonly Func<TValue, string> prefixSupplier;

	private BonusAdmission(Func<TValue, string> prefixSupplier) {
		this.prefixSupplier = prefixSupplier;
	}

	public string GetPrefix(TValue value) => prefixSupplier.Invoke(value);

	static BonusAdmission() {
		if (typeof(TValue) == typeof(int)) {
			Add = new BonusAdmission<TValue>(value => Convert.ToInt32(value) > 0 ? "+" : "");
			AddAndSubtract = Add;
		} else if (typeof(TValue) == typeof(float)) {
			Add = new BonusAdmission<TValue>(value => Convert.ToSingle(value) > 0f ? "+" : "");
			AddAndSubtract = Add;
		} else {
			UnityEngine.Debug.LogWarning($"Unexpected BonusAdmission type {typeof(TValue)}.");
			Add = None;
			AddAndSubtract = None;
		}
	}
}