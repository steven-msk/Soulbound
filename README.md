# Soulbound - Prototype

This repository contains the early-stage prototype of Soulbound, a solo-developed RPG inspired by Terraria, Hollow Knight and Undertale.
It serves as the foundation for gameplay systems, architecture, and experimentation before moving into the full production phase.

---

## Project Status

This is **not** a production-ready project.
All code, systems, and structures are exerimental and subject to heavy future changes. The purpose is to explore:
- Core mechanics
- Input and interaction models
- UI/UX experimentation
- Data structures and gameplay flow
- Gamedev features

Expect:
- Rapid iteration
- Spaghetti code in places
- Untested systems
- Incomplete or placeholder content
- Frequent task comments
- Messy commits (im not the best with git)

---

## Built with

- **Unity Engine 6000.0.37f1** (URP 2D, new Input System)
- Custom input/event context management system
- Modular components for UI, entity control, and world interaction

---

## External libraries used (not maintained by Unity)
- Zenject 9.2.0

## Notes

- Branching strategy may be used later - currently all changes live in `prototype` branch until probject stabilization.
- This repo may be squashed or restructured before transitioning to production.

## License

Private development. License TBD closer to production.

---  

# Developer notes

## Task tokens

The following tokens have been introduced for developer use to improve codebase clarity and task management:
- `TODO` (Priority: Normal) - used for tasks that need to be implemented or fixed soon. These tasks are relatively high-priority.
- `FUTURE TODO` (Priority: Low) - indicates a potential upcoming change or design consideration. Similar to `TODO`, but it's not urgent.
- `FEATUREIMPL` (Priority: High) - indicates a feature that has not yet been implemented but is planned for the near future
- `REMINDER` (Priority: Low) - used to leave non-critical notes or consideration about future reference.
- `PLANNED` (Priority: High) - indicates a feature or change that is already decided on, but not implemented yet. Its something you know you want to add, just at a later time.
- `REFACTOR` (Priority: Normal) - indicates code that works correctly but could be improved for better readability or maintainability.
- `POTENTIAL` (Priority: Normal) - indicates a possible feature, rework, or refactor that is under consideration but not yet confirmed.  
- `REWORK` (Priority: High) - indicates that a feature needs deep changes - both game visuals, gameplay, and code logic may be affected by this change

Making combination of task tokens is possible and actually recommended. They help clarify an idea or a future commit in the codebase:
- `POTENTIAL FEATUREIMPL` - indicates a feature implementation that is being considered but has not yet confirmed.
- `PLANNED REFACTOR` - a refactor that is already decided on, but not implemented yet.

---

<!---
incoherent capability definitions: capabilities are not instantiatable and arent referenced in each item definition

## Item Capabilities and Effect Definitions

- The central interface for all item behaviors is `IItemCapability`. <b>This is a marker interface and must not be mocked or implemented directly.</b>
- The base class for use-triggered item behaviors is `ItemUseEffect`. <b>This class is abstract and should not be instantiated or referenced directly.</b> To define behavior, you must inherit from it.

When introducing a new item behavior, create a new interface that inherits from `IItemCapability`, and optionally register it inside ItemUsageHandler to link capability types with behavior mappings.  
Defining a new use effect should be done using these steps:
1. Create an abstract effect class inherited from `ItemUseEffect`, then add fields, shared logic, and any custom behavior. You may override the `Execute()` method, but this is not recommended - it will never be called outside of the class's context.
2. Create the class containing the effect implementation that inherits from your abstract effect class.

Then assign the effect to an item:
1. Create a class that inherits both `Item` and the new capability interface
2. Create an asset of the effect class
3. Assign the effect asset to the item`s effect field (make sure both the field and the item asset exist first)

The following example is taken from an actual feature in the game's code, but may be outdated due to ongoing development:



#### Capability definition
```csharp
public interface IConsumable : IItemCapability {
	public int ConsumeAmount { get; }
	public ConsumableEffect ConsumeAction { get; }

	public virtual void Consume(ItemStack itemStack) {
		ConsumeAction?.OnConsume(this, itemStack);
		itemStack.Quantity -= ConsumeAmount;
	}
}
```

#### Effect definition
```csharp
public abstract class ConsumableEffect : ItemUseEffect {
	public abstract void OnConsume(IConsumable consumable, ItemStack itemStack);
}
```

#### Item asset reference
```csharp
[CreateAssetMenu(menuName = "Items/ConsumableItem")]
public class ConsumableItem : Item, IConsumable {
	[CanBeNull] [SerializeField] private ConsumableEffect consumeAction;

	public ConsumableEffect ConsumeAction => consumeAction;

	[SerializeField] private int consumeAmount;
	public int ConsumeAmount => consumeAmount;

	protected override AbstractTooltip GetDefaultTooltip() {
		return CompoundTooltip.Of(TooltipData.Concat((base.GetDefaultTooltip() as CompoundTooltip).Data.ToArray(), Tooltip.Tag(ItemTag.Consumable).Data));
	}
}
```

#### Effect asset reference
```csharp
[CreateAssetMenu(fileName = "ConsumableEffect_test", menuName = "Items/Effects/ConsumableEffect_test")]
public class ConsumableEffect_test : ConsumableEffect {
	public override void OnConsume(IConsumable consumable, ItemStack itemStack) {
		Debug.Log($"consumed {consumable.ConsumeAmount}, remaining: {itemStack.Quantity}");
	}
}
```

#### Use effect registration
```csharp
itemUsageHandler.Register<IConsumable>(ItemUseTrigger.LeftClick, (consumable, stack) => consumable.Consume(stack));
```
-->





<!---
may be moved in code documentation files

## BufferedStat implementation

A `BufferedStat` is a special kind of `SerializableStat` that only applies its effects conditionally, based on external triggers defined through `IBufferedTrigger`. This interface acts as a "stat wrapper" that becomes active or inactive depending on event-based or time-based rules.

#### Fields:
- `IBufferedTrigger applyBufferedTrigger`:  
Trigger responsible for determining when the stat should be applied (e.g. after a delay, when a condition is met)
- `IBufferedTrigger revokeBufferedTrigger`:  
Trigger responsible for determining when the stat should be revoked or unapplied.

#### Methods:
```csharp
public void EnableBuffers(IStatProvider source)
```
- Binds both the apply and revoke triggers to a stat source
- Typically called when the stat is activated in the system

```csharp
public void DisableBuffers(IStatProvider source)
```
- Unbinds both the apply and revoke triggers from the stat source
- Typically called when the stat is being removed or the provider is disposed

### `IBufferedTrigger`

An interface defining how a buffered stat determined when it should apply or revoke its effects. Acts as an abstraction over time-based, event-passed, or condition-based triggers that "buffer" the actual stat changes.

#### Properties:

```csharp
BufferedTriggerState State { get; set; }
```
- The target action of the trigger (Apply or Revoke)
- Used for state control - <b> Do not change the value at runtime unless specifically needed to</b>

```csharp
Func<bool> InvocationValidator { get; }
```
- A validation delegate to check whether the trigger is allowed to invoke its logic (e.g. avoid redundant invocation or enfore preconditions)

#### Methods:

```csharp
void Enable(BufferedStat stat, IStatProvider source)
```
- Binds the trigger to a `BufferedStat` and its source
- Usually hooks into an event system or schedules logic for deferred invocation

```csharp
void Disable(BufferedStat stat, IStatProvider source)
```
- Unbinds or cancels the trigger's logic or event subscriptions

```csharp
void Invoke(BufferedStat stat, Action action)
```
- Executes the trigger logic, wrapping a given action (typically applying or revoking the stat)
- This allows deferred or conditionally controlled stat application

### Conceptual notes:
- `BufferedStat` provides declarative and serializable control over when a stat is considered active
- `IBufferedTrigger` provides reactive control, bridging the stat info with gameplay logic (when/how it applies)
- This abstraction is ideal for temporary buffs/debuffs, event-driven stat effects (e.g. on hit, on move), and environmental or conditional effects
- Triggers are modular and interchangeable. You can compose complex behavior by mixing different implementations (e.g. `TimedBufferedTrigger` with `EventTimerBufferedTrigger` and so on)
-->