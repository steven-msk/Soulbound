using System;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundBackend.Common.Patterns {
	public static class Implementations {

		public static List<Type> GetAllImplementationsOf<T>() {
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assemblies => assemblies.GetTypes())
				.Where(type => typeof(T).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
				.ToList();
		}

		public static List<Type> GetAllImplementationsOf(Type typeImpl) {
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assemblies => assemblies.GetTypes())
				.Where(type => typeImpl.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
				.ToList();
		}
	}
}
