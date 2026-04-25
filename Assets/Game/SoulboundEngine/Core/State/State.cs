using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SoulboundEngine.Core.States {
	public abstract class State<O, S> {
		private readonly Entries entries;
		protected O owner;
		private Dictionary<Property, Dictionary<object, S>> withTable = new();

		public State(O owner, Entries entries) {
			this.owner = owner;
			this.entries = entries;
		}

		public bool Contains<T>(Property<T> property) where T : IComparable<T> {
			return this.entries.ContainsKey(property);
		}

		public void CreateWithTable(Dictionary<Dictionary<Property, object>, S> states) {
			Dictionary<Property, Dictionary<object, S>> table = new();

			foreach (var property in this.entries.Keys) {
				table[property] = new Dictionary<object, S>();

				foreach ((Dictionary<Property, object> propertyMap, S state) in states) {
					bool isSibling = this.entries.Keys
						.Where(p => p != property)
						.All(p => propertyMap.TryGetValue(p, out object v) 
									&& v.Equals(this.entries[p]));
					if (isSibling) {
						table[property][propertyMap[property]] = state;
					}
				}
			}

			this.withTable = table;
		}

		public T Get<T>(Property<T> property) where T : IComparable<T> {
			return (T)this.entries.GetValueOrDefault(property);
		}

		public bool TryGet<T>(Property<T> property, out T value) where T : IComparable<T> {
			bool result = this.entries.TryGetValue(property, out object boxed);
			value = result ? (T)boxed : default;
			return result;
		}

		public ReadOnlyDictionary<Property, object> GetEntries() => this.entries;

		public IEnumerable<Property> GetProperties() => this.entries.Keys;

		public S With<T, V>(Property<T> property, V value) where V : T where T : IComparable<T> {
			if (this.withTable.TryGetValue(property, out Dictionary<object, S> byValue)) {
				if (byValue.TryGetValue(value, out S state)) {
					return state;
				}
			}
			throw new ArgumentException($"Block state not found: {property}, value: {value}. Maybe you forgot to override AppendProperties?");
		}

		public override string ToString() {
			return $"state[owner={this.owner}, entries={this.entries}, withTable=[{string.Join(", ", this.withTable)}]]";
		}

		public sealed class Entries : ReadOnlyDictionary<Property, object> {
			public Entries(IDictionary<Property, object> entries) 
				: base(entries) {
			}

			public override string ToString() {
				return $"[{string.Join(", ", this)}]";
			}
		}
	}
}
