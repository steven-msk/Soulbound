using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Obsolete]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate | AttributeTargets.Class, Inherited = false)]
public sealed class EventContextHandlerAttribute : System.Attribute {
	public string ContextName { get; }

	public EventContextHandlerAttribute(string contextName) {
		this.ContextName = contextName;
	}
}
