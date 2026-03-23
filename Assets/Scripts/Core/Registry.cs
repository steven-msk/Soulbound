using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace SoulboundBackend.Core {
	public static class Registry<T> {
		private static readonly Dictionary<IRegistrationKey<T>, T> registry = new();
		private static IRegistrationContract<T, IRegistrationKey<T>> contract;

		public static TValue Add<TValue>(TValue value) where TValue : T {
			if (contract == null) throw ContractNotSet();

			IRegistrationKey<T> key = contract.ValueToKey(value);
			if (!registry.ContainsKey(key)) registry.Add(key, value);
			return value;
		}

		public static IRegistrationContract<T, IRegistrationKey<T>> SetContract(IRegistrationContract<T, IRegistrationKey<T>> contract) {
			Registry<T>.contract = contract;
			return contract;
		}

		public static bool TryGet(IRegistrationKey<T> key, out T value) {
			if (contract == null) throw ContractNotSet();

			return registry.TryGetValue(key, out value);
		}

		public static void Remove(IRegistrationKey<T> key) => registry.Remove(key);

		public static IEnumerable<T> GetAll() => registry.Values;

		private static NotSupportedException ContractNotSet() {
			return new($"Registry contract not set for type {typeof(T)}");
		}
	}
}
