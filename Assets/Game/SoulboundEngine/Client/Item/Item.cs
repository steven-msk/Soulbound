using SoulboundEngine.Client.Component;
using SoulboundEngine.Client.World.Block;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public class Item : IItemConvertible {
		public const int DEFAULT_FULL_STACK = 256;
		public static readonly Dictionary<Block, Item> blockItems = new();
		private readonly Settings settings;
		private readonly IComponentMap components;

		public Item(Settings settings) {
			this.settings = settings;
			this.components = settings.components.Build();
		}

		public string name => this.settings.name;
		public int fullStackSize => this.settings.fullStackSize;
		public bool IsStackable() => this.settings.IsStackable();

		public void AppendToBlock(Block block) {
			blockItems.Add(block, this);
		}

		public virtual ItemStack GetDefaultStack(int count = 1) {
			return new ItemStack(this, Mathf.Clamp(count, 0, this.fullStackSize));
		}

		public override string ToString() {
			return Items.GetIdentifier(this)?.ToString() ?? base.ToString();
		}

		public Item AsItem() => this;

		public IComponentMap GetComponents() => this.components;

		public sealed class Settings {
			public readonly IComponentMap.Builder components = IComponentMap.Create();
			public string name { get; private set; }
			public int fullStackSize { get; private set; } = DEFAULT_FULL_STACK;

			private Settings(string name, int fullStackSize) {
				this.name = name;
				this.fullStackSize = fullStackSize;
			}

			public static Settings Of(string name) {
				return new Settings(name, DEFAULT_FULL_STACK);
			}

			public Settings NonStackable() {
				this.fullStackSize = 1;
				return this;
			}

			public Settings StackUpTo(int count) {
				this.fullStackSize = count;
				return this;
			}

			public Settings Component<T>(ComponentType<T> component, T value) {
				this.components.Add(component, value);
				return this;
			}

			public static Settings Air() {
				return new("Air", 1);
			}

			public bool IsStackable() => this.fullStackSize > 1;
		}
	}
}
