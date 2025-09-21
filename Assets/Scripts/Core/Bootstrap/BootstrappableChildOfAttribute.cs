using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BootstrappableChildOfAttribute : Attribute, IBootstrappableNodeHandle {
		public Type Dependency { get; }
	
		public BootstrappableChildOfAttribute(Type dependency) {
			Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
		}
	}
}
