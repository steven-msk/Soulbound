using SoulboundBackend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SoulboundBackend.Core.Bootstrap {
	public class BootstrapTreeBuilder {
		private readonly Dictionary<Type, IBootstrappable> registry;

		public BootstrapTreeBuilder(IEnumerable<IBootstrappable> bootstrappables) {
			registry = bootstrappables.ToDictionary(b => b.GetType());
		}

		public List<IBootstrappable> BuildTree<THandle>(Type startType) where THandle : Attribute, IBootstrappableNodeHandle {
			var result = new List<IBootstrappable>();
			BuildRecursive(startType, result, new HashSet<Type>(), typeof(THandle));
			return result;
		}

		private void BuildRecursive(Type type, List<IBootstrappable> list, HashSet<Type> visited, Type handle) {
			if (visited.Contains(type)) {
				return;
			}
			visited.Add(type);

			var requires = type.GetCustomAttributes(handle, true).Cast<IBootstrappableNodeHandle>();
			foreach (var req in requires) {
				BuildRecursive(req.Dependency, list, visited, handle);
			}
			list.Add(registry[type]);
		}
	}
}
