using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum BufferedTriggerCondition {
	AlwaysTrue,
	AlwaysFalse
}

public static class BufferedTriggerConditionRegistry {
	private static readonly Dictionary<BufferedTriggerCondition, Func<bool>> conditions = new() {
		[BufferedTriggerCondition.AlwaysTrue] = () => true,
		[BufferedTriggerCondition.AlwaysFalse] = () => false,
	};

	public static Func<bool> ToValidator(this BufferedTriggerCondition condition) {
		if (conditions.TryGetValue(condition, out var validator)) {
			return validator;
		} else {
			Debug.LogError($"BufferedTriggerCondition not found: {condition}. This should really not happen...");
		}
		return conditions[BufferedTriggerCondition.AlwaysTrue];
	}
}