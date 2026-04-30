using SoulboundEngine.Client.ItemSystem.Render;
using SoulboundEngine.Client.World.BlockSystem;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.ItemSystem {
	public class Item {
		public const int DEFAULT_FULL_STACK = 256;
		public static readonly Dictionary<Block, Item> blockItems = new();
		private readonly Settings settings;

		public Item(Settings settings) {
			this.settings = settings;
		}

		public string name => this.settings.name;
		public int fullStackSize => this.settings.fullStackSize;
		public bool IsStackable() => this.settings.IsStackable();

		public void AppendToBlock(Block block) {
			blockItems.Add(block, this);
		}

		public ItemRenderData GetRenderData(ItemStack itemStack) {
			return this.settings.renderFunction(itemStack);
		}

		public virtual ItemStack CreateStack(int quantity = 1) {
			return new ItemStack(this, Mathf.Clamp(quantity, 0, this.fullStackSize));
		}

		public sealed class Settings {
			public string name { get; private set; }
			public int fullStackSize { get; private set; } = DEFAULT_FULL_STACK;
			public Func<ItemStack, ItemRenderData> renderFunction { get; private set; }

			private Settings(string name, int fullStackSize, Func<ItemStack, ItemRenderData> renderFunction) {
				this.name = name;
				this.fullStackSize = fullStackSize;
				this.renderFunction = renderFunction;
			}

			public static Settings Of(string name, Func<ItemStack, ItemRenderData> renderFunction) {
				return new Settings(name, DEFAULT_FULL_STACK, renderFunction);
			}

			public Settings NonStackable() {
				this.fullStackSize = 1;
				return this;
			}

			public Settings StackUpTo(int count) {
				this.fullStackSize = count;
				return this;
			}

			public static Settings Air() {
				return new("Air", 1, _ => default);
			}

			public static Func<ItemStack, ItemRenderData> RenderFunction(string spriteKey, Identifier? animation = null) {
				return itemStack => new ItemRenderData(spriteKey, itemStack, animation);
			}

			public bool IsStackable() => this.fullStackSize > 1;
		}
	}
}
