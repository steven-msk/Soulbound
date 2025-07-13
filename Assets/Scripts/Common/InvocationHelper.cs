using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class InvocationHelper {
#nullable enable
	public static void NullOrElse<T>(this T? actionTarget, Action<T> action, Action fallback) where T : class {
		if (actionTarget != null) {
			action(actionTarget);
		} else {
			fallback();
		}
	}

	public static void InvokeIf(this Action? action, Func<bool>? predicate) {
		if (predicate?.Invoke() ?? true) {
			action?.Invoke();
		}
	}

	public static void InvokeIf(this Action? action, bool condition) {
		if (condition) {
			action?.Invoke();
		}
	}

	public static void InvokeIf<T>(this Action<T>? action, T arg, Func<bool>? predicate) {
		if (predicate?.Invoke() ?? true) {
			action?.Invoke(arg);
		}
	}

	public static void InvokeIf<T>(this Action<T>? action, T arg, bool condition) {
		if (condition) {
			action?.Invoke(arg);
		}
	}

	public static void InvokeIf<T1, T2>(this Action<T1, T2>? action, T1 arg1, T2 arg2, Func<bool>? predicate) {
		if (predicate?.Invoke() ?? true) {
			action?.Invoke(arg1, arg2);
		}
	}

	public static void InvokeIf<T1, T2>(this Action<T1, T2>? action, T1 arg1, T2 arg2, bool condition) {
		if (condition) {
			action?.Invoke(arg1, arg2);
		}
	}

	public static void InvokeIf<T1, T2, T3>(this Action<T1, T2, T3>? action, T1 arg1, T2 arg2, T3 arg3, Func<bool>? predicate) {
		if (predicate?.Invoke() ?? true) {
			action?.Invoke(arg1, arg2, arg3);
		}
	}

	public static void InvokeIf<T1, T2, T3>(this Action<T1, T2, T3>? action, T1 arg1, T2 arg2, T3 arg3, bool condition) {
		if (condition) {
			action?.Invoke(arg1, arg2, arg3);
		}
	}

	public static void InvokeIf<T1, T2, T3, T4>(this Action<T1, T2, T3, T4>? action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Func<bool>? predicate) {
		if (predicate?.Invoke() ?? true) {
			action?.Invoke(arg1, arg2, arg3, arg4);
		}
	}

	public static void InvokeIf<T1, T2, T3, T4>(this Action<T1, T2, T3, T4>? action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, bool condition) {
		if (condition) {
			action?.Invoke(arg1, arg2, arg3, arg4);
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