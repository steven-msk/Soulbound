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
- Frequent TODO, REMINDER, and FIXME comments

## Goals for Prototype Phase

- Lay down the foundation systems for input, inventory, combat, world stability
- Explore soul-binding mechanics and item interactions
- Establish a flexible architecture which can support heavy content expansion later

---

## Built with

- **Unity Engine 6000.0.37f1** (URP 2D, new Input System)
- Custom input/event context management system
- Modular components for UI, entity control, and world interaction

---

## Notes

- Branching strategy may be used later - currently all changes live in `prototype` branch until probject stabilization.
- This repo may be squashed or restructured before tansitioning to production.

## License

Private development. License TBD closer to production.


---

---

# Implementation notes
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

Example (custom tooltip to set the tooltip text to the current stack number)
Developers: You can find a template tooltip creation snippet in `Documentation/Snippets/CustomTooltipTemplate`.

```csharp
[CreateAssetMenu(menuName = "Test/CustomTooltip")]
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

