using SoulboundEngine.Client.Component;
using SoulboundEngine.Client.Interaction;
using SoulboundEngine.Client.Player;
using SoulboundEngine.Client.World.Block;
using SoulboundEngine.Client.World.Entity;
using SoulboundEngine.Client.World.Level;
using SoulboundEngine.Core.Registry;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace SoulboundEngine.Client.Item {
	public class Item : IItemConvertible {
		public const int DEFAULT_FULL_STACK = 256;
		public static readonly Dictionary<Block, Item> blockItems = new();
		private readonly RegistryKey<Item> registryKey;
		private readonly IComponentMap components;

		protected Item(Settings settings) {
			// localization not supported yet
			this.components = settings.Build(settings.GetTranslationKey());
			this.registryKey = settings.registryKey ?? throw new NotSupportedException("Item is not added to a registry");
		}

		public static Item Create(Settings settings) {
			return new Item(settings);
		}

		public string GetName() => this.components.Get(ItemComponents.NAME);

		public int GetMaxCount() => this.components.Get(ItemComponents.MAX_STACK_COUNT);

		public bool IsStackable() => this.GetMaxCount() > 1;

		public Item AsItem() => this;

		public IComponentMap GetComponents() => this.components;

		protected void AppendToBlock(Block block) {
			blockItems.Add(block, this);
		}

		public virtual ItemStack GetDefaultStack(int count = 1) {
			return new ItemStack(this, Mathf.Clamp(count, 0, this.GetMaxCount()));
		}

		public RegistryEntry<Item> GetRegistryEntry() => Items.GetEntry(this.registryKey);

		public override string ToString() {
			return this.GetRegistryEntry().GetIdAsString();
		}

		/// <summary> 
		/// Called when the player starts using the item (left click).
		/// This method is called last in the entity -> block -> air dispatch order,
		/// which means if this method was called then both <see cref="OnPrimaryUseOnBlock(BlockInteractionResult)"/>
		/// and <see cref="OnPrimaryUseOnEntity(ItemStack, PlayerEntity, Entity)"/> have passed, or the interaction was made directly in air.
		/// </summary>
		public virtual IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) => IActionResult.PASS;

		/// <summary>
		/// Called when the player starts using the item (left click) if targeting a block.
		/// If this method was called, it means that the interaction was made directly on a block, or 
		/// <see cref="OnPrimaryUseOnEntity(ItemStack, PlayerEntity, Entity)"/> has passed.
		/// </summary>
		public virtual IActionResult OnPrimaryUseOnBlock(BlockInteractionResult result) => IActionResult.PASS;

		/// <summary>
		/// Called when the player starts using the item (left click) if targeting an entity.
		/// This is always the first call in the entity -> block -> air dispatch order.
		/// If this passes, then <see cref="OnPrimaryUseOnBlock(BlockInteractionResult)"/> is called.
		/// </summary>
		public virtual IActionResult OnPrimaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) => IActionResult.PASS;

		/// <summary> 
		/// Called when the player starts using the item (right click)
		/// This method is called last in the entity -> block -> air dispatch order,
		/// which means if this method was called then both <see cref="OnSecondaryUseOnBlock(BlockInteractionResult)"/>
		/// and <see cref="OnSecondaryUseOnEntity(ItemStack, PlayerEntity, Entity)"/> have passed, or the interaction was made directly in air.
		/// </summary>
		public virtual IActionResult OnSecondaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) => IActionResult.PASS;

		/// <summary> 
		/// Called when the player starts using the item (right click) targeting a block 
		/// If this method was called, it means that the interaction was made directly on a block, or 
		/// <see cref="OnSecondaryUseOnEntity(ItemStack, PlayerEntity, Entity)"/> has passed.
		/// </summary>
		public virtual IActionResult OnSecondaryUseOnBlock(BlockInteractionResult result) => IActionResult.PASS;

		/// <summary> 
		/// Called when the player starts using the item (right click) targeting an entity.
		/// This is always the first call in the entity -> block -> air dispatch order.
		/// If this passes, then <see cref="OnSecondaryUseOnBlock(BlockInteractionResult)"/> is called.
		/// </summary>
		/// </summary>
		public virtual IActionResult OnSecondaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) => IActionResult.PASS;

		public sealed class Settings {
			private readonly IComponentMap.Builder components = IComponentMap.Create().AddAll(ItemComponents.DEFAULT_COMPONENTS);
			internal RegistryKey<Item>? registryKey;

			public IComponentMap Build(string name) {
				this.components.Add(ItemComponents.NAME, name);
				return this.components.Build();
			}

			public Settings NonStackable() => this.StackUpTo(1);

			public Settings StackUpTo(int count) {
				this.components.Add(ItemComponents.MAX_STACK_COUNT, count);
				return this;
			}

			public Settings Component<T>(ComponentType<T> component, T value) {
				this.components.Add(component, value);
				return this;
			}

			public Settings RegistryKey(RegistryKey<Item> key) {
				this.registryKey = key;
				return this;
			}

			/// <summary>
			/// Must be called after setting the registry key
			/// </summary>
			/// <exception cref="InvalidOperationException"></exception>
			internal string GetTranslationKey() {
				if (this.registryKey is null) {
					throw new InvalidOperationException("Cannot derive item name: RegistryKey was not set before Build() was called.");
				}

				return $"item.{this.registryKey.value.GetNamespace()}.{this.registryKey.value.GetPath()}";
			}
		}
	}
}
