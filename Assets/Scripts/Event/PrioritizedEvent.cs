using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;

public class PrioritizedEvent {
	public string Context { get; }
	public int Priority { get; }
	public HashSet<string> Allows { get; }

	public PrioritizedEvent(string context, int priority, params string[] allowedContexts) {
		Context = context ?? throw new ArgumentNullException(nameof(context));
		Allows = new HashSet<string>(allowedContexts) ?? null;
		Priority = priority;
	}

	public bool AllowsContext(string context) => Allows?.Contains(context) ?? false;
	
	public bool AllowsContext(PrioritizedEvent other) => Allows?.Contains(other.Context) ?? false;

	public override string ToString() => $"EventContext({Context}, Allows: {Allows.ToArrayPooled()})";
}
