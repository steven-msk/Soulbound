using SoulboundBackend.Client.ItemSystem;
using SoulboundBackend.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

#nullable enable

namespace SoulboundBackend.Client.World.BlockSystem {
	public abstract partial class Block {
		private static readonly Logger logger = Logger.CreateInstance();
		public abstract string name { get; }
		public abstract TileBase tileReference { get; }
		public abstract BlockItem? itemReference { get; }
		public virtual BreakRequirement? breakRequirement => null;

		protected Dictionary<IBlockStateProperty, object> propertyMap = new();
		public IList<IBlockStateProperty> propertyDefinitions => propertyMap.Keys.AsReadOnlyList();
		public bool propertyDefinitionTerminated { get; protected set; } = false;

		public BlockState defaultState { get; private set; }
		protected Dictionary<int, BlockState> cachedStates = new();

		protected Block() {
			RegisterProperties();
			propertyDefinitionTerminated = true;
			RegisterDefaultState(CreateDefaultState());
			InitializePredefinedStates();
		}

		protected abstract void RegisterProperties();
		protected abstract BlockState CreateDefaultState();
		protected virtual void InitializePredefinedStates() {
		}

		public void RegisterProperty<T>(BlockProperty<T> property, T defaultValue) {
			if (propertyDefinitionTerminated) {
				logger.LogWarning(null, "Cancelled block property registration '{}' due to definition context termination", property.name);
				return;
			}
			propertyMap.Add(property, defaultValue!);
		}

		public object GetDefaultValueOfProperty(IBlockStateProperty property) {
			if (!this.HasProperty(property)) {
				return null!;
			}
			return propertyMap[property];
		}

		protected void RegisterDefaultState(BlockState state) {
			defaultState = state;
			cachedStates[ComputeHash(state)] = state;
		}

		public BlockState GetStateFor(BlockStateProperties properties) {
			int hash = ComputeHash(properties);

			if (!cachedStates.TryGetValue(hash, out var state)) {
				Dictionary<IBlockStateProperty, object> cloned = new();
				cloned.AddRange(properties.ToList());

				state = new BlockState(this, cloned, CreateBehaviorFor(properties));
				cachedStates[hash] = state;
			}
			return state;
		}

		public bool TryGetStateByHash(int hash, out BlockState state) {
			return cachedStates.TryGetValue(hash, out state);
		}

		public bool HasProperty(IBlockStateProperty property) {
			return propertyMap.ContainsKey(property);
		}

		public BlockState WithProperty<T>(BlockState state, BlockProperty<T> property, T value) {
			return state.With(property, value);
		}

		protected virtual IBlockStateBehavior CreateBehaviorFor(BlockStateProperties properties) {
			return defaultState.stateBehavior;
		}

		internal int ComputeHash(object obj) => obj.GetHashCode();

		public override string ToString() {
			return name;
		}
	}
}