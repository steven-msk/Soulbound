using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
	public sealed class DependencyContainer {
		private readonly Dictionary<Type, object> dependencies = new();

		public void Register<T>(T instance) where T : class {
			var type = typeof(T);
			if (dependencies.ContainsKey(type)) {
				throw new InvalidOperationException($"Dependency of type {type} is already registered.");
			}
			dependencies[type] = instance ?? throw new ArgumentNullException(nameof(instance));
		}

		public T Resolve<T>() {
			return dependencies.TryGetValue(typeof(T), out var instance) ? (T)instance : default;
		}
	}
}