using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InvocationHelper {
#nullable enable
	public static void InvokeNullableOrElse<T>(this T? actionTarget, Action<T> action, Action fallback) where T : class {
		if (actionTarget != null) {
			action(actionTarget);
		} else {
			fallback();
		}
	}

	public static void IfElse(bool condition, Action success, Action fail) {
		if (condition) {
			success.Invoke();
		} else {
			fail.Invoke();
		}
	}

	public static void If(bool condition, Action action) {
		if (condition) {
			action.Invoke();
		} 
	}

	public static void PatternIf<T>(object obj, Action<T> success) {
		if (obj is T patternTarget) {
			success.Invoke(patternTarget);
		}
	}
}