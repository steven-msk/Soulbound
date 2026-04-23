## Top overview
- an entity is an object inside a world with floating-point position and attributes, behavior and can interact with other world elements
- its responsible for adding gameplay state to the world, where each entity contributes with its own behavior and attributes.
- every `Entity` is characterized by its `EntityDescriptor`, and a `Guid` assigned when the entity is first added to a world. Every entity also defines an `IEntityTransform` used for spatial context.
- at runtime, an entity is added to a world and is assigned a guid. From there the entity responds to updates and executes its behavior until it is removed. An entity can implement `ITickingEntity` to be able to define ticking logic.
- relies on the block and item system as some features may depend on block state properties or item stack data, on the registry system because every `EntityDescriptor` must be registered, and on the attribute system as they provide important data.
- extension points include extending the transform type to support different spatial manipulation, physics, as well as compontent-based features and multiple transform-related features.
- known weaknesses:
    - transform creation design is underdeveloped due to lack of features
    - missing `IEntityTransform` capabilities due to lack of feature
    - unavailable update scheduling also due to lack of features
    - inconsistent entity lifetime logic
    - attributes are not yet integrated
    - possible terminology issues between entity "disposal", "removal", "despawn"
    - no serialization

## Entity instances and descriptors
- an `EntityDescriptor` is a registered object that an entity represents at runtime. It defines the default creation process for its target entity and is always deterministic.
- every live entity has a guid assigned when the entity is added *first* in a world.
- a `Level` is the sole authority over entity lifecycle, update scheduling, spatial indexing, and guid assignment. An entity cannot exist in multiple levels at the same time. An entity can mutate its own membership to a level *(but currently unavailable because of the wrong `Dispose` implementation)*.
- an entity can be created through `EntityDescriptor.Create(Level, Vector2)`, which creates and adds an entity of that type to the world according to the `EntityFactory` passed to the descriptor.
- another way to create an entity is by using the entity's constructor, but that completely disregards any setup steps that the factory passed in the `EntityDescriptor` that may be important for the entity's state. **Only use the constructor if you know that there will never be any special setup for that entity.**
- an entity that is created through a constructor **must be added manually to a world**. If an entity has been created but isnt added to any world (transform is missing), it is considered a "dormant" entity, and it should be used for serialization-only state as these instances are dangerous. A dormant entity can have its data mutated but any transform-related calls will throw an exception. A dormant entity may be re-added to a world, in which case the same guid is used and any data mutations that occurred prior to re-addition do not get reverted.
- *there is an active indecision about whether or not allow "dormant" entities as read-only states. Reusing guids across entity lifecycle may create identity ambiguity. It is considered to make entities unavailable after their lifecycle is over, and throw an exception on any calls made on such instances. One fix would be to "pack" the entity data, decoupling from the entity instance, and reuse it somewhere else, but the guid cannot be retained this way, which is an acceptable compromise. For now this can be passed to prod, but keep in mind that this may need stabilization during prod.*

## Transforms
- an entity's transform essentially validates its existence in a world, and is responsible for keeping the spatial context accessible across the lifetime of an entity.
- the transform is created strictly by the entity, and its implementation varies based on the type of the entity. *(this needs a rework as it may leak Unity-side code and is underdeveloped)*
- after an entity is created with a constructor (no matter through what path), the entity needs to be added to a world in order for the entity to be considered alive, otherwise its an inactive entity.

## Inactive vs. dormant vs. alive clarification
- there is a small window where a new entity is created, but hasnt been added to a world yet, so no transform and no guid exists for that instance. This instance is considered "inactive", as it is not identifiable because it doesnt have a guid.
- its important to note that a **dormant entity != inactive entity**. Dormant entities always have a guid assigned, but no transform, while inactive entities dont have either.
- an entity is considered alive if it has a transform and guid assigned. These are managed by a `Level` and can be disposed at any time.
- note that `Dispose()` currently only destroys the transform, but it may still be managed by a `Level`. **This is not intentional and must be fixed before prod.**

## Attributes
- an attribute is a double-precision number that can be stored on an entity.
- every entity contains an `AttributeContainer` which allows attribute instances to be created and used for the entity's state logic.
- every `AttributeInstance` defined in an attribute container is characterized by its type and a default value, as well as modifiers that change the final value.
- attribute modifiers are objects that target a specific attribute and are characterized by three things: an identifier, a value and the operation which is a numerical function applies the value to another value. An attribute modifier may also provide an optional target which indicates where this modifier should apply its value. By default the target is null which means it targets this attribute directly.
- attribute modifiers may also be added with a predicate, a function that checks whether this modifier should apply or not. The predicate is evaluated at compute-time, and is updated frequently to keep modifers and dirty state consistent.
- *currently attributes are not integrated within the entity implementation, but will have a use before prod*

## Known weaknesses
- transform creation design is underdeveloped due to lack of features. The current flow looks somewhat like: "entity created -> add to world (assign id) -> create transform", where the transform is created by the entity itself. This isnt really scalable, and because the transform acts as sort of a bridge between the backend form of an `Entity` and Unity's Game Object code, it allows Game Object code to leak in `CreateTransform()`, breaking backend code consistency.
    - a suitable replacement would be to remove transform creation entirely from `Entity` and move it to a registry. Then `Entity` can provide an identifier or a key to the registered transform and request an instance. This would completely invalidate any leaks regarding transform creation through Game Objects into the backend code.
- missing `IEntityTransform` capabilities due to lack of features. Physics-related calls could be made outside of `Entity` but the transform doesnt provide any way to access physics directly, even though there is a `PhysicsTransform` implementation which uses Unity's Rigidbody2D which can be pulled up in `IEntityTransform`.
- inconsistent lifetime logic: `Dispose()` only destroys the transform, but doesnt post the removed state to `Level`. Besides that, there isnt any clear separation between a managed and an unmanaged instance. There is `IsAlive()` which retrieves whether this entity is alive or not, but that doesnt guarantee that it isnt managed, only that the transform isnt available.
- no attribute integration
- possible terminology issues between entity "disposal", "removal", "despawn". Since entities arent fully developed yet, and their lifetime isnt 100% deterministic, there could be some confusion between what entity "disposal" vs "removal" means, or whether an entity getting disposed is the same as getting despawned.
- no serialization due to lack of features and use of cross-session entities besides the player

## Must be addressed and fixed before prod
- transform creation design
- `IEntityTransform` capabilities
- entity lifetime logic
- update scheduling (ticks, frame updates)
- attribute integration
- possible terminology issues
- serialization