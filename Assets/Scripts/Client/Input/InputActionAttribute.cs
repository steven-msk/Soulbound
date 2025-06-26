using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public class InputActionAttribute : System.Attribute {
	public string Context { get; }
	public int Priority { get; set; }

	public InputActionAttribute(string context) {
		Context = context;
	}
}
