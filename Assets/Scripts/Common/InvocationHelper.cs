using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InvocationHelper {
#nullable enable
	public static void InvokeOrElse<T>(this T? actionTarget, Action<T> action, Action fallback) where T : class {
		if (actionTarget != null) {
			action(actionTarget);
		} else {
			fallback();
		}
	}
}