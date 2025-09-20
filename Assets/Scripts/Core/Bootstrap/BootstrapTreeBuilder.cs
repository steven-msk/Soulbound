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

		public List<IBootstrappable> BuildTree<TIndicator>(Type startType) where TIndicator : Attribute, IBootstrappableNodeIndicator {
			var result = new List<IBootstrappable>();
			BuildRecursive(startType, result, new HashSet<Type>(), typeof(TIndicator));
			return result;
		}

		private void BuildRecursive(Type type, List<IBootstrappable> list, HashSet<Type> visited, Type indicatorType) {
			if (visited.Contains(type)) {
				return;
			}
			visited.Add(type);

			var requires = type.GetCustomAttributes(indicatorType, true).Cast<IBootstrappableNodeIndicator>();
			foreach (var req in requires) {
				BuildRecursive(req.Dependency, list, visited, indicatorType);
			}
			list.Add(registry[type]);
		}
	}
}
