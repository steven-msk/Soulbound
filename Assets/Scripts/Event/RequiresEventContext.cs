using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Obsolete]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Delegate, Inherited = false)]
public sealed class RequiresEventContextAttribute : System.Attribute {
	public string ContextName { get; }
	public int TargetPriority { get; }

	public RequiresEventContextAttribute(string contextName, int targetPriority) {
		this.ContextName = contextName;
		this.TargetPriority = targetPriority;
	}
}
