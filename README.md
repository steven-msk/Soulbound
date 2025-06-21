# Soulbound - Prototype

This repository contains the early-stage prototype of Soulbound, a solo-developed RPG inspired by games like Terraria, Hollow Knight and Undertale.
It serves as the foundation for gameplay systems, architecture, and experimentation before moving into the full production phase.

---

## Project Status

This is **not** a production-ready project.
All code, systems, and structures are exerimental and subject to heavy future changes. The purpose is to explore:
- Core mechanics like inventory, item usage, and event systems
- Input and interaction models
- UI/UX experimentation
- Data structures and gameplay flow

Expect:
- Rapid iteration
- Spaghetti code in places
- Untested systems
- Incomplete or placeholder content
- Frequent TODO, REMINDER, FIXME, FEATUREIMPL and REFACTOR comments

## Goals for Prototype Phase

- Lay down the foundation systems for input, inventory, combat, world stability
- Explore soul-binding mechanics and item interactions
- Establish a flexible architecture which can support heavy content expansion later

---

## Built with

- **Unity Engine 6000.0.37f1** (URP 2D, new Input System)
- Custom input/event context management system
- Modular components for UI, entity control, and world interaction

## Notes

- Branching strategy may be used later - currently all changes live in `prototype` branch until probject stabilization.
- This repo may be squashed or restructured before tansitioning to production.

## License

Private development. License TBD closer to production.

---  

# Developer notes

## Task tokens

The following tokens have been introduced for developer use to improve codebase clarity and task management:
- `FEATUREIMPL` (Priority: High) - indicates a feature that has not yet been implemented but is planned for the near future
- `REMINDER` (Priority: Low) - used to leave non-critical notes or consideration about future reference.
- `REFACTOR` (Priority: Normal) - indicates code that works correctly but could be improved for better readability or maintainability.

---

## Custom item tooltips

Architecture overview:
- `AbstractTooltipSerialzier`: ScriptableObject, assigned per Item, returns a `ITooltipSerializer`
- `ITooltipSerializer`: return the actual runtime `AbstractTooltip`
- `AbstractTooltip`: the tooltip instance, handles show/hide/update logic

Custom item tooltip implementation should be done using these steps:
1. Create a new class extending `AbstractTooltip` or an existing tooltip implementation (e.g. `Tooltip`), implement `Update()` and other logic as needed
2. Create a class `ITooltipSerializer`, return your tooltip instance
3. Create a `ScriptableObject` extending `AbstractTooltipSerializer` and return your serializer
4. Assign the ScriptableObject to an `Item`    

Remember to create a ScriptableObject tooltip asset found in:
```csharp
[CreateAssetMenu(menuName = "Items/Custom Tooltips/YourCustomTooltipNameHere")] 
```
Then assign the created asset to the desired item.

Example (custom tooltip to set the tooltip text to the current stack number)
Developers: You can find a template tooltip creation snippet in `Documentation/Snippets/CustomTooltipTemplate`.

```csharp
[CreateAssetMenu(menuName = "Items/Custom Tooltips/CustomTooltip")]
public class CustomTooltipSerializer : AbstractTooltipSerializer {
	public override ITooltipSerializer GetSerializer(Item item) {
		return new CustomTooltipData();
	}
}

public class CustomTooltipData : ITooltipSerializer {
	public AbstractTooltip Generate() {
		return new CustomTooltip(Tooltip.Plain("0").Data);
	}
}

public class CustomTooltip : Tooltip {

	public CustomTooltip(TooltipData data) : base(data) {
	}

	public override void Update(ItemStack itemStack) {
		base.Update(itemStack);
		data.Text = itemStack.Quantity.ToString();
		if (tooltipPanel != null) {
			tooltipPanel.GetComponentInChildren<TextMeshProUGUI>().text = data.Text;
		}
	}
}
```
---

## Custom Weapon Attack Behaviors

This system uses `AbstractWeaponAttackBehavior` as the foundation for custom attack logic through code.
Custom behaviors should be created using these steps:
1. Create a new ScriptableObject extending `AbstractWeaponAttackBehavior`, then override or implement your custom logic
2. Assign an instance of the newly created behavior to the desired WeaponItem.  

#### Important note:
`AbstractWeaponAttackBehavior` should not define direct motion logic, as movement should be controlled by the animation clip and its assigned behavior. Instead, use `AbstractWeaponAttackBehavior` to define events that occur before, during, or after the attack is executed.  

However, an exception may be made in cases where the animation clip does not contain a `StateMachineBehavior` or other mechanism to handle motion. In such cases, motion-relateed behavior may be implemented within `AbstractWeaponAttackBehavior`. <b> This approach is not recommended, as it breaks the separation between visual timing and gameplay behavior, and can lead to desynchronized or hard-to-maintain attack logic.</b>
As a general rule, motion should remain animation-driven, and `AbstractWeaponAttackBehavior`s should be reserved for handling state changes, audio or visual feedback, or gameplay triggers.

Events that occur during the attack animation can be triggered through either animation clip events (defined directly in the animation timeline), or external sources.  
When invoking animation events, you must call:
```csharp
AttackHandler.InvokeAnimationEvent(string eventName);
```
where `eventName` must match the name of the action previously registered in the `AbstractWeaponAttackBehavior`:
```csharp
public override Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier => new() {
	["eventName"] = attackHandler => { /* custom logic */ }
};
```

Here is an example of custom weapon attack behavior (weapon's movement is defined in a custom `StateMachineBehavior`)
```csharp
[CreateAssetMenu(menuName = "Items/Attack Behaviors/WeaponAttackBevahior_test")]
public class WeaponAttackBehavior_test : AbstractWeaponAttackBehavior {
	public override Dictionary<string, Action<AttackHandler>> AnimationEventsSupplier => new() {
		["AnimEvent"] = _ => Debug.Log("event")
	};

	public override void Setup(AttackHandler attackHandler) { 
		attackHandler.transform.position = GameManager.GetPlayerInstance().transform.position;
	}

	public override void PostAttack(AttackHandler attackHandler) {
		base.PostAttack(attackHandler);
		Debug.Log("destroy");
	}
}
```

