using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class InputActionRequest {
	public string Context { get; private set; }
	public int Priority { get; private set; }
	public Action Action { get; private set; }

	public InputActionRequest(string context, int priority, Action action) {
		Context = context;
		Priority = priority;
		Action = action;
	}
}