using SoulboundBackend.Core.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Core.Bootstrap {
    public sealed class BootstrappableInstanceFactory {
        private readonly Dictionary<Type, Func<IBootstrappable>> suppliers = new();

        public void Register<T>(Func<T> supplier) where T : IBootstrappable {
            suppliers[typeof(T)] = () => (T)supplier.Invoke();
        }

        public void Override<T>(Func<T> supplier) where T : IBootstrappable {
            if (!suppliers.TryGetValue(typeof(T), out var existingSupplier)) {
                Register<T>(supplier);
                return;
            }
            existingSupplier = () => supplier.Invoke();
        }

        public IBootstrappable Create(Type type) {
            if (suppliers.TryGetValue(type, out var supplier)) {
                return supplier.Invoke();
            }
            throw new InvalidOperationException($"No supplier registered for type {type}");
        }

        public T Create<T>() where T : IBootstrappable {
            return (T)Create(typeof(T));
        }
    }
}

