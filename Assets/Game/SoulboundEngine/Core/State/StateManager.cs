using SoulboundEngine.Client.Debug.Logging;
using SoulboundEngine.Common;
using System.Collections.Generic;
using System.Linq;

namespace SoulboundEngine.Core.States {
	public class StateManager<O, S> where S : State<O, S> {
		public delegate S Factory(O owner, Dictionary<Property, object> entries);

		private readonly SortedDictionary<string, Property> properties;
		private readonly IReadOnlyList<S> states;
		private readonly O owner;

		public StateManager(O owner, Factory factory, Dictionary<string, Property> propertyMap) {
			this.properties = new SortedDictionary<string, Property>(propertyMap);
			this.owner = owner;

			List<Dictionary<Property, object>> combinations = new();

			Logger.LogInfo("doing cartesian product for: {}", owner);

			// create individual property sets
			foreach (var property in propertyMap.Values) {
				IEnumerable<object> values = property.GetValues();
				foreach (var value in values) {
					combinations.Add(new Dictionary<Property, object>() {
						[property] = value
					});
				}
			}

			// cartesian product over each individual set
			foreach (var propertySet in combinations.ToList()) {
				foreach (var candidateSet in combinations.ToList()) {
					if (candidateSet.Equals(propertySet)) continue;

					foreach (var (property, value) in candidateSet) {
						if (propertySet.ContainsKey(property)) continue;

						propertySet.Add(property, value);
					}
				}
			}

			Logger.LogInfo("finished cartesian product for: {}", owner);

			// one state per combination
			List<S> allStates = combinations
				.Select(entries => factory(owner, entries))
				.ToList();

			// build with table and seal
			Dictionary<Dictionary<Property, object>, S> statesByEntries = allStates
				.ToDictionary(s => s.GetEntries().ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
			foreach (var state in allStates) {
				state.CreateWithTable(statesByEntries);
			}

			this.states = allStates;
			this.defaultState = this.states.FirstOrDefault() ?? factory(owner, Dictionaries.Empty<Property, object>());
		}

		public S defaultState { get; set; }

		public O GetOwner() => this.owner;

		public IEnumerable<Property> GetProperties() => this.properties.Values;

		public IReadOnlyList<S> GetStates() => this.states;

		public override string ToString() {
			return $"state_manager[owner={this.owner}, properties=[{string.Join(", ", this.properties)}]]";
		}

		public sealed class Builder {
			private readonly Dictionary<string, Property> namedProperties = new();
			private readonly O owner;

			public Builder(O owner) {
				this.owner = owner;
			}

			public Builder Add(params Property[] properties) {
				foreach (var property in properties) {
					this.namedProperties.Add(property.name, property);
				}
				return this;
			}

			public StateManager<O, S> Build(Factory factory) {
				return new StateManager<O, S>(this.owner, factory, this.namedProperties);
			}
		}
	}
}
