namespace SoulboundEngine.Item {
	using SoulboundEngine.Component;
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

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
			return new ItemStack(this, Math.Clamp(count, 0, this.GetMaxCount()));
		}

		public RegistryEntry<Item> GetRegistryEntry() => Items.GetEntry(this.registryKey);

		public override string ToString() {
			return this.GetRegistryEntry().GetIdAsString();
		}

		public int GetBreakLevel() => this.components.GetOrDefault(ItemComponents.BREAK_LEVEL, 0);

		// TODO: implement inventory ticking across all IInventory implementations
		[Obsolete]
		public virtual void InventoryTick(Level level, Entity owner, ItemStack stack, EquipmentSlot? slot) {
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

		/// <summary> Called when the use timer reached 0 </summary>
		public virtual ItemStack OnUseFinished(ItemStack stack, InteractionType type, Level level, Entity user) => stack;

		/// <summary> Called when the player finished using this item. This is immediately after any of the OnPrimary/OnSecondaryUse methods. </summary>
		public virtual ItemStack OnItemUsed(ItemStack stack, InteractionType type, Level level, Entity user) => stack;

		/// <summary> Returns how many ticks this item's interaction spans. </summary>
		public virtual int GetUseTime(ItemStack stack, InteractionType type, Level level, Entity user) => 0;

		/// <summary> 
		/// Called every tick while this item is being used.
		/// Should return the stack that should replace the current one, or <c>stack</c> if there is no change
		/// </summary>
		public virtual ItemStack OnUseTick(ItemStack stack, InteractionType type, Level level, Entity user, int remainingTicks) => stack;

		/// <summary> 
		/// Called when the player has stopped using this item for various reasons before reaching the target use time.
		/// Should return the stack that should replace the current one, or <c>stack</c> if there is no change.
		/// Interactions can be canceled from any source that overwrites the in-progress stack. 
		/// If this is the case, then the stack returned by this may be immediately replaced by the other incoming stack.
		/// </summary>
		public virtual ItemStack OnUseCanceled(ItemStack stack, InteractionType type, Level level, Entity user, int remainingTicks) => stack;

		/// <summary> 
		/// Called when the item use is canceled or the interaction is finished. 
		/// The resulting stack is a byproduct of the returned stack and the one from either OnUseCanceled or OnUseFinished, 
		/// depending on which one was called, so <paramref name="stack"/> is the ItemStack returned by those methods.
		/// If OnUseFinished was called, <paramref name="remainingTicks"/> is equal to <c>0</c>.
		/// </summary>
		public virtual ItemStack OnUseCanceledOrFinished(ItemStack stack, InteractionType type, Level level, Entity user, int remainingTicks) => stack;

		/// <summary>
		/// Returns whether the player can continue using this item the tick after this item has been used. 
		/// This is false by default.
		/// </summary>
		public virtual bool ShouldContinueUse(ItemStack stack, InteractionType type, Level level, PlayerEntity player, BlockPos blockPos) => false;

		public int GetDurability() => this.components.GetOrDefault(ItemComponents.DURABILITY, int.MaxValue);

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

			public Settings BreakLevel(int breakLevel) {
				return this.Component(ItemComponents.BREAK_LEVEL, breakLevel);
			}

			public Settings Durability(int durability) {
				return this.Component(ItemComponents.DURABILITY, durability);
			}

			/// <summary>
			/// Must be called after setting the registry key
			/// </summary>
			/// <exception cref="InvalidOperationException"></exception>
			internal string GetTranslationKey() {
				return this.registryKey is null
					? throw new InvalidOperationException("Cannot derive item name: RegistryKey was not set before Build() was called.")
					: this.registryKey.value.ToTranslationKey("item");
			}
		}
	}
}
