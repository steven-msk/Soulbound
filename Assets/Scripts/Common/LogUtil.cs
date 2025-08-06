using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LogUtil {
	public static void LogAwake(MonoBehaviour script) {
		Debug.Log($"<color=cyan>[AWAKE]</color> {script.GetType().Name} on '{script.gameObject.name}'");
	}
}
