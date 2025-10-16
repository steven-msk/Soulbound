using SoulboundBackend.Client.World.BlockSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SoulboundBackend.Common {
    public interface ICachedRegistry<T> where T : class, IHashableReference {
        protected static ConcurrentDictionary<int, T> cached = new();
        protected static ConcurrentDictionary<int, Func<T>> cachedReferences = new();

        protected static T Lookup(string key, Func<T> instanceSupplier) {
            int hash = HashHelper.StableHash(key);

            return (T)cached.GetOrAdd(hash, hashedID => {
                T instance = instanceSupplier.Invoke();
                instance.hashedID = hashedID;
                return instance;
            });
        }

        protected static void RegisterCachedReference<TAttr>(TAttr cacheAttribute, PropertyInfo property)
                where TAttr : Attribute, ICachedReferenceAttribute {
            var getter = property.GetGetMethod();
            if (getter == null) {
                throw new NotSupportedException("No getter found for block property: " + property);
            }

            Func<T> accessor = () => (T)getter.Invoke(null, null);
            int hash = HashHelper.StableHash(cacheAttribute.propertyName);
            cachedReferences[hash] = accessor;
        }

        public static IDictionary<int, T> GetCachedRegistry() {
            return cached;
        }

        public static IDictionary<int, Func<T>> GetCachedReferences() {
            return cachedReferences;
        }
    }
}
