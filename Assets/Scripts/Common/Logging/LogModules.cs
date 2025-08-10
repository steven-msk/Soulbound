using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class LogModules {
	public static readonly LogModule awake = new LogModule("AWAKE", "#00FFFF");
	public static readonly LogModule resource = new LogModule("RESOURCE", "#32CD32");
}
