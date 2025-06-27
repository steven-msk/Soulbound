using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class BufferedConditionRegistry {
	private static readonly Dictionary<string, Func<bool>> conditions = new() {
		["AlwaysTrue"] = () => true,
		["AlwaysFalse"] = () => false,
	};

	public static Func<bool> GetCondition(string methodName) {
		if (conditions.TryGetValue(methodName, out var condition)) {
			return condition;
		}
		if (!string.IsNullOrEmpty(methodName)) {
			Debug.LogWarning($"BufferedConditionRegistry: No condition for '{methodName}'");
			return () => false;
		}
		return conditions["AlwaysTrue"];
	}
}