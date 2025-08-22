using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class EntitySpawnPropertyCandidatesAttribute : Attribute {
	public string[] candidates { get; }

	public EntitySpawnPropertyCandidatesAttribute(params string[] candidates) {
		this.candidates = candidates;
	}
}