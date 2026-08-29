namespace SoulboundEngine.Item {
	using Newtonsoft.Json.Linq;
	using SoulboundEngine.Component;
	using SoulboundEngine.Interaction;
	using SoulboundEngine.Registry;
	using SoulboundEngine.World.Block;
	using SoulboundEngine.World.Entity;
	using SoulboundEngine.World.Entity.Attribute;
	using SoulboundEngine.World.Level;
	using SoulboundEngine.World.Player;
	using System;
	using System.Collections.Generic;

#nullable enable

	public struct ItemStack : IComponentHolder {
		private static MergedComponentMap cachedEmptyComponents = null!;
		private static MergedComponentMap CachedEmptyComponents => cachedEmptyComponents ??= MergedComponentMap.Create(Items.AIR.GetComponents(), ComponentChanges.EMPTY);
		public static readonly ItemStack EMPTY = new();
		private readonly MergedComponentMap components;
		private readonly Item item;
		public int count { get; private set; }

		// components are null on a default(ItemStack) instance
		// to guard against this, read this instead of this.components directly
		// if AssertComponentChangesNotOnEmpty is called, then its safe to use this.components
		private readonly MergedComponentMap ComponentsNonNull => this.components ?? CachedEmptyComponents;

		public ItemStack(IItemConvertible item)
			: this(item, 1) {
		}

		public ItemStack(IItemConvertible item, int count)
			: this(item, count, ComponentChanges.EMPTY){
		}

		public ItemStack(IItemConvertible item, int count, ComponentChanges componentChanges)
			: this (item, count, MergedComponentMap.Create(item.AsItem().GetComponents(), componentChanges)) {
		}

		public ItemStack(RegistryEntry<Item> item) 
			: this(item, 1) {
		}

		public ItemStack(RegistryEntry<Item> item, int count) 
			: this(item.GetValue(), count) {
		}

		private ItemStack(IItemConvertible item, int count, MergedComponentMap components) {
			this.item = item.AsItem();
			this.count = count;
			this.components = components;
		}

		public readonly IComponentMap GetComponents() => this.ComponentsNonNull;

		public readonly ComponentChanges GetComponentChanges() => this.ComponentsNonNull.AsPatch();

		public readonly IComponentMap GetDefaultComponents() => this.GetItem().GetComponents();

		public readonly void ApplyChanges(ComponentChanges changes) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.SetChanges(changes);
		}

		public readonly void SetComponentsFrom(IComponentMap map) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.SetAll(map);
		}

		public readonly void Set<T>(ComponentType<T> type, T defaultValue, Func<T, T> applier) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue)));
		}

		public readonly void Set<T, U>(ComponentType<T> type, T defaultValue, U change, Func<T, U, T> applier) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, applier(this.components.GetOrDefault(type, defaultValue), change));
		}

		public readonly void ResetToDefaultComponents() {
			this.AssertComponentMutationNotOnEmpty();
			this.components.ClearChanges();
		}

		public readonly void AppendTooltip(List<string> tooltips) {
			Item item = this.GetItem();
			MergedComponentMap components = this.ComponentsNonNull;
			tooltips.Add(item.GetName());

			if (components.Contains(ItemComponents.DURABILITY)) {
				int durability = components.Get(ItemComponents.DURABILITY);
				tooltips.Add($"Durability: {durability}");
			}
		}

		public readonly bool IsFull() => this.count >= this.item.GetMaxCount();
		public readonly bool IsEmpty() => this.count <= 0 || this.item == null || Equals(this.item, Items.AIR);

		public readonly bool IsFullSize(int count) => count >= this.item.GetMaxCount();

		/// <summary>
		/// Try to add items. Returns how may were actually added.
		/// </summary>
		public int Increment(int amount = 1) {
			if (amount <= 0) return 0;

			int added = Math.Min(this.GetSpaceLeft(), amount);
			this.count += added;
			return added;
		}
		
		/// <summary>
		/// Try to remove items. Returns how many were actually removed
		/// </summary>
		public int Decrement(int amount = 1) {
			if (amount <= 0) return 0;

			int removed = Math.Min(this.count, amount);
			this.count -= removed;
			return removed;
		}

		/// <summary> 
		/// Returns a copy of this item stack with the count decremented by amount,
		/// EMPTY if the new stack is empty, or a copy of this stack if the amount is less or equal to 0.
		/// </summary>
		public readonly ItemStack DecrementBy(int amount) {
			if (amount <= 0) return this;

			int newCount = Math.Max(0, this.count - amount);
			return newCount <= 0 ? EMPTY : this.CopyWithCount(newCount);
		}

		public readonly int GetSpaceLeft() {
			return this.IsOf(null) ? 0 : this.item.GetMaxCount() - this.count;
		}

		public readonly bool IsOf(Item? item) {
			return item == null ? this.IsEmpty() : Equals(item, this.item);
		}

		public static bool AreItemsEqual(ItemStack a, ItemStack b) {
			return a.IsOf(b.item) && b.IsOf(a.item);
		}

		public static bool AreItemsAndComponentsEqual(ItemStack a, ItemStack b) {
			return AreItemsEqual(a, b) && a.ComponentsNonNull.Equals(b.ComponentsNonNull);
		}

		public static bool AreEqual(ItemStack a, ItemStack b) {
			return a.count == b.count && AreItemsAndComponentsEqual(a, b);
		}

		public void FillFrom(ref ItemStack itemStack) {
			if (!AreItemsEqual(itemStack, this)) return;

			int added = itemStack.Decrement(this.GetSpaceLeft());
			this.Increment(added);
			if (itemStack.IsEmpty()) itemStack = EMPTY;
		}

		public ItemStack Split(int amount) {
			int actualAmount = this.Decrement(amount);
			return actualAmount <= 0 ? EMPTY : this.CopyWithCount(amount);
		}

		public readonly ItemStack CopyWithCount(int newCount) {
			return this.IsOf(null) ? EMPTY : new ItemStack(this.item, newCount, this.components.Copy());
		}

		public readonly ItemStack Copy() => this.CopyWithCount(this.count);

		public readonly ItemStack CopyFullStack() {
			return this.IsOf(null) ? EMPTY : this.CopyWithCount(this.item.GetMaxCount());
		}

		public ItemStack CopyAndEmpty() {
			ItemStack stack = this;
			this.count = 0;
			return stack;
		}

		public readonly ItemStack CopyComponentsToNewStack(IItemConvertible item, int count) {
			this.AssertComponentMutationNotOnEmpty();
			return new ItemStack(item, count, this.components.Copy());
		}

		public readonly void Copy<T>(ComponentType<T> type, IComponentsAccess from) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(type, from.Get(type));
		}

		public readonly T Set<T>(ComponentType<T> type, T? value) {
			this.AssertComponentMutationNotOnEmpty();
			if (value == null) return this.Remove(type);
			this.components.Set(type, value);
			return value;
		}

		public readonly T Remove<T>(ComponentType<T> type) {
			this.AssertComponentMutationNotOnEmpty();
			return this.components.Remove(type);
		}

		public void CapCount(int maxCount) {
			this.count = Math.Clamp(this.count, 0, maxCount);
		}

		public readonly Item GetItem() => this.item ?? Items.AIR;

		public readonly int GetMaxCount() => this.GetItem().GetMaxCount();

		public readonly void ForEachAttributeModifier(EquipmentSlot slot, Action<RegistryEntry<AttributeType>, AttributeModifier> consumer) {
			ItemAttributeModifiers modifiers = this.GetOrDefault(ItemComponents.ATTRIBUTE_MODIFIERS, ItemAttributeModifiers.EMPTY);
			modifiers.ForEach(slot, consumer);
		}

		public readonly int GetBreakLevel() {
			return this.ComponentsNonNull.GetOrDefault(ItemComponents.BREAK_LEVEL, this.GetItem().GetBreakLevel());
		}

		public readonly void InventoryTick(Level level, Entity owner, EquipmentSlot? slot) {
			this.GetItem().InventoryTick(level, owner, this, slot);
		}

		public static IActionResult OnPrimaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return stack.GetItem().OnPrimaryUse(stack, level, player, blockPos);
		}
		public static IActionResult OnPrimaryUseOnBlock(ItemStack stack, BlockInteractionResult result) {
			return stack.GetItem().OnPrimaryUseOnBlock(result);
		}
		public static IActionResult OnPrimaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) {
			return stack.GetItem().OnPrimaryUseOnEntity(stack, player, target);
		}
		public static IActionResult OnSecondaryUse(ItemStack stack, Level level, PlayerEntity player, BlockPos blockPos) {
			return stack.GetItem().OnSecondaryUse(stack, level, player, blockPos);
		}
		public static IActionResult OnSecondaryUseOnBlock(ItemStack stack, BlockInteractionResult result) {
			return stack.GetItem().OnSecondaryUseOnBlock(result);
		}
		public static IActionResult OnSecondaryUseOnEntity(ItemStack stack, PlayerEntity player, Entity target) {
			return stack.GetItem().OnSecondaryUseOnEntity(stack, player, target);
		}

		public readonly ItemStack OnItemUsed(InteractionType type, Level level, Entity user) {
			return this.GetItem().OnItemUsed(this, type, level, user);
		}

		public readonly ItemStack OnUseCanceled(InteractionType type, Level level, Entity user, int remainingTicks) {
			ItemStack stack = this.GetItem().OnUseCanceled(this, type, level, user, remainingTicks);
			return this.GetItem().OnUseCanceledOrFinished(stack, type, level, user, remainingTicks);
		}

		public readonly ItemStack OnUseTick(InteractionType type, Level level, Entity user, int remainingTicks) {
			return this.GetItem().OnUseTick(this, type, level, user, remainingTicks);
		}

		public readonly ItemStack OnUseFinished(InteractionType type, Level level, Entity user) {
			ItemStack stack = this.GetItem().OnUseFinished(this, type, level, user);
			return this.GetItem().OnUseCanceledOrFinished(stack, type, level, user, 0);
		}

		public readonly int GetUseTime(InteractionType type, Level level, Entity user) {
			return this.GetItem().GetUseTime(this, type, level, user);
		}

		public readonly bool ShouldContinueUse(InteractionType type, Level level, PlayerEntity player, BlockPos blockPos) {
			return this.IsEmpty() || this.GetItem().ShouldContinueUse(this, type, level, player, blockPos);
		}

		public readonly bool HasDurability() {
			return this.ComponentsNonNull.Contains(ItemComponents.DURABILITY); 
		}

		public readonly bool IsDamaged() {
			return this.HasDurability() && this.GetCurrentDurability() < this.GetMaxDurability(); 
		}

		public readonly int GetCurrentDurability() {
			return !this.HasDurability() ? int.MaxValue : this.ComponentsNonNull.Get(ItemComponents.DURABILITY);
		}

		public readonly int GetMaxDurability() {
			return !this.HasDurability() ? int.MaxValue : this.GetItem().GetDurability();
		}

		public readonly void SetDurability(int amount) {
			this.AssertComponentMutationNotOnEmpty();
			this.components.Set(ItemComponents.DURABILITY, Math.Clamp(amount, 0, this.GetMaxDurability()));
		}

		public readonly bool IsBroken() => this.GetCurrentDurability() <= 0;

		public void DamageAndBreak(int amount, Entity owner, EquipmentSlot slot) {
			this.DamageAndBreak(amount, brokenItem => owner.OnEquippedItemBroke(brokenItem, slot));
		}

		public void DamageAndBreak(int amount, Action<Item> onBreak) {
			int newAmount = this.ProcessDurabilityChange(amount);
			if (newAmount != 0) {
				this.ApplyDamage(amount, onBreak);
			}
		}

		public void DamageWithoutBreaking(int amount) {
			int newAmount = this.ProcessDurabilityChange(amount);
			if (newAmount == 0) return;

			if (this.GetCurrentDurability() - newAmount <= 0) return;
			this.ApplyDamage(amount, _ => { });
		}

		private readonly int ProcessDurabilityChange(int amount) {
			return !this.HasDurability() ? 0 : amount;
		}

		private void ApplyDamage(int damage, Action<Item> onBreak) {
			if (!this.HasDurability()) return;

			this.SetDurability(this.GetCurrentDurability() - damage);
			if (this.IsBroken()) {
				Item item = this.GetItem();
				this.Decrement(1);
				onBreak(item);
			}
		}

		public static JToken ToJson(ItemStack stack) {
			return stack.IsEmpty()
				? JValue.CreateNull()
				: new JObject() {
					["item"] = Items.GetIdentifier(stack.GetItem()).ToString(),
					["count"] = stack.count,
					["changes"] = ComponentChanges.ToJson(stack.GetComponentChanges())
				};
		}

		public static ItemStack FromJson(JToken json) {
			if (json.Type == JTokenType.Null) return EMPTY;
			if (json.Type != JTokenType.Object) {
				Logger.LogError("ItemStack json is not object: {}", json);
				return EMPTY;
			}

			string? itemIdString = (string?)json["item"];
			if (itemIdString == null) {
				Logger.LogError("No item property in stack json: {}", json);
				return EMPTY;
			}

			Identifier itemId = Identifier.Of(itemIdString);
			Item? item = Items.Get(itemId);
			if (item == null) {
				Logger.LogError("Unknown item: {}", itemId);
				return EMPTY;
			}

			int? count = (int?)json["count"];
			if (count == null) {
				Logger.LogError("No count property in stack json: {}", json);
				return EMPTY;
			}

			JToken componentsToken = json["changes"] ?? JValue.CreateNull();
			ComponentChanges components = ComponentChanges.FromJson(componentsToken);
			return new ItemStack(item, count.Value, components);
		}

		private readonly void AssertComponentMutationNotOnEmpty() {
			if (this.IsEmpty()) throw new NotSupportedException("Cannot mutate components on empty stack");
		}

#pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

		[Obsolete("Cannot compare two item stacks with Equals", true)]
		public override bool Equals(object obj) {
			throw new NotSupportedException("Cannot compare two item stacks with Equals");
		}

		public readonly override int GetHashCode() {
			return HashCode.Combine(this.item, this.count, this.components);
		}

		public readonly override string ToString() {
			return this.IsEmpty() ? "EMPTY" : $"{this.item}[{this.count}]";
		}
	}
}
