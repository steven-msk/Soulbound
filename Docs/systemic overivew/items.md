## Top overview
- an item is an object that players and other entities can use
- an item is responsible for handling logic and doesnt hold runtime data. Any runtime data is stored on an item stack.
- `Item` is the class that represents an item, instances of this do not hold any runtime data. To make use of data for an item use an `ItemStack`. Currently the only information that `Item` holds is the max stack count and a name.
- at runtime an `ItemStack` can be stored in a slot, inside a container such as the player's inventory. `ItemStack` can forward item-related event handles to its representing `Item`.
- relies on the block system as some items can be converted into their block form (`BlockItem`, *not yet implemented*), on the registry system as every `Item` instance must be registered.
- unknown evolution standards (for now)
- known weaknesses: 
    - poor `Item` definition due to lack of features
    - `ItemStack` contains no meaningful data besides stack quantity (and a placeholder `IItemStackData`)
    - item usage design and interactions (cross-system) might be too vague and overcomplicated, stack creation is slightly inconsistent
    - current implementation of stack quantity mutation might be dangerous
    - slight problem with stack identity
    - risk of desync between slot operations
    - missing stack compatibility rules

## Registration
- every `Item` instance **must** be registered to `Registries.ITEMS`, using one unique identifier. Two instances of the same item that share logic can be created but must be registered with different ids.
- items are registered once at registration time, at bootstrap. Once an item is registered it cannot be removed.

## Item vs. ItemStack instances
- an `Item` is a stateless object and globally registered inside `Registries.ITEMS`
- an `ItemStack` is owner by exactly one system at a time, like a slot, transit (cursor), temporary operation buffer etc.
- data stored in `Item` apply to all `ItemStack` instances that represent it.
- `ItemStack` is not immutable and can have its data changed at runtime. Instances of `ItemStack`s are not registered and no are not suitable for caching as their data can be mutated in a number of ways. An `ItemStack` is the same as another if both share the same item and retain the same data including the stack quantity. *(known issue: slight problem with stack identity)*
- an `Item` does not own any `ItemStack`s created with it. An `ItemStack` can only represent one item at a time. The quantity of a stack must be strictly in the range [0, itemMaxStackCount]. It is recommended to dispose of an `ItemStack` instance and not reuse it if it ever reaches quantity 0, as in most cases the visuals are destroyed when this happens. *(known issue: current implementation stack quantity mutation might be dangerous)*
- an `ItemStack` can be created through `Item.CreateStack(int)` *(known issue: stack creation ambiguity)*

## Item usage and interactions
- subclasses of `Item` can implement usage capabilities, such as `IContainerItemListener`, `IPlaceableItem` *(which will turn into `BlockItem`)* etc, which are all subclasses of `IItemInteractionListener`.
- these can be handled by an `IInteractionHandler<T>`, where `T` is an interaction context. An item interaction context is represented by `ItemInteraction`. Every item interaction has a trigger and the stack that was interacted with.
- the interaction handler is reponsible for deciding whether a given interaction context can be executable, as well as execute it with the respective context. An `InteractionResolver` is used to handle multiple interaction handlers for different contexts. As an example, `Player` implements `IInteractionHandler<ItemInteraction>` as well as `IInteractionHandler<BlockInteraction>`, and relies on an `InteractionResolver` to resolve actions made within the context of both blocks and items. If an `ItemStack` reaches its max count its considered a full stack and is generally unreponsive to any other additions.
- in the case of items, the interaction handler implementation should invoke execution for any `IItemInteractionListener` that matches the given context and environmental condition *(known issue: item usage design and interactions might be too vague and overcomplicated)*

## Containers
- `ItemStack` instances can be stored inside slots, which are objects specifically made to retain stacks. Slots make up a container, for example the player's inventory.
- a slot's stack can be monitored through events like `stackChanged` and `quantityChanged`, but be aware of stack instance disposal when stacks reach quantity 0, as mentioned previously.
- a slot either holds a valid `ItemStack` with quantity > 0, or is empty (stack == null)
- a slot can only represent one container across its lifetime.
- each slot is assigned an index which represents its position in the container
- container must never share `ItemStack` references across slots (because of the shared `ItemStack` instance problem)

## Stack operations
- inside a container, a number of operations can be applied to one or multiple slots at a time.
- the stack which is assigned a capability of following the mouse cursor is called a transit stack, and its not owned by any slot.
- a few examples of operations for the player's inventory include `CollectAllItemsToTransit`, `SwapTransit`, `TransferSingleToSlot` etc.
- some considerations to keep in mind:
    - the transit `ItemStack` should not be shared with other slots, as any mutations made on the stack would also reflect on the slot, leading to bugs.
    - operations may need a specific context based on their requirements. Any attempt on an operation execution with an invalid context will result in the operation not executing at all.
    - operations themselves are created by the container in which a slot interaction happened. Based on the trigger and current context, the container should resolve the correct operation which can be executed with the given input and operation target.
    - cross-container contexts originate in the world screen object.

## Known issues:
- poor `Item` definition due to lack of features. Because of the missing friction between features at runtime, the current `Item` implementation doesnt meet the requirements for a prod baseline.
- `ItemStack` contains no meaningful data besides stack quantity and a placeholder `IItemStackData`. Again because of the missing features, there isnt developed any data retaining design besides the placeholder `Dictionary<Type, IItemStackData>` which retains the data by its type and can be accessed freely.
- item usage design and interactions might be too vague and overcomplicated. The current design focuses on the fact that interactions can happen non-deterministically with unpredictable contexts, and every item may expose a totally different approach to an interaction type that would break the design. The current implementation aims to reduce that non-deterministic factor. However, it might as well be too complex even for a prod version, because interaction types are limited and contexts should be predictable even without knowing it fully, and extending the types shouldnt break even for a static implementation. This doesnt apply only to items, it applies to the interaction system as a whole.
    - a considerable fix would be to drop `InteractionResolver` and all `IInteractionHandler<T>`s, then have each item expose their interaction type based on the given stack. Interaction resolution can then be made separately and logic can be executed independently by whatever initiated the interaction. In the case of multiple types of interactions happening at the same time (multiple targets), they can also be resolved by whoever initiated the interaction(s).
- stack creation is slightly inconsistent. Currently, stacks should be created using `Item.CreateStack(int)`, because items may infer data that the item has by default inside an `ItemStack` instance. But the `ItemStack` constructor is only made internal, and even though it is flagged that it should only be used by item's CreateStack method, its still accessible. In production this inconsistency would get annoying.
    - a fix would be to make `ItemStack` constructor private, and force every stack creation to go through the item in order to append the default data to the stack. This would also allow an optional `Item.GetDefaultStack` implementation.
- current implementation of stack quantity mutation might be dangerous. `ItemStack` is a class, so method passes are by reference to the instance. This means that if any 2 sources share the same `ItemStack` instance, they can both mutate the quantity (and data by default). By convention, `ItemStack` instances should never be cached, and should be treated as DTOs, but this may not behave well in prod. If a stack reaches 0 and is getting disposed, event listeners may still reference the instance and UI may still hold a pointer.
    - a possible fix would be to make every `ItemStack` instance immutable, and any changes made to an instance would return a totally different instance instead. This would deduplicate the data mutation, but it may create new problems with instances if they are not handled carefully. This is not really necessary for prod though.
- slight problem with stack identity. As said previously, an `ItemStack` is the same as another if both share the same item and retain the same data including the stack quantity. This might break during prod because of the instability between stacks not being able to differentiate to one another. Consider adding a read-only id to it so that comparison can be made easier.
- risk of desync between slot operations. `ISlotOperation` exposes both `CanExecute` and `Execute`, both of which are used to first check if the operation can be executed with a given context and actually execute it. Consider combining them into a `TryExecute` implementation which returns a result. The result must reflect exactly why the operation can or cannot execute.
- missing stack compatibility rules because of missing features

## Must be addressed and fixed before prod:
- `Item` definition
- `ItemStack` data retaining
- interaction design (as a whole)
- stack creation inconsistency
- shared stack quantity and data instances
- stack identity
- slot operation desync risk
- stack compatibility rules
