## Top overview:
- a block is the structural unit which gives shape to a world.
- a block is responsible for defining logic and properties for a block state in the world
- `Block` is the class that represents a type of block. The actual object used inside the world is `BlockState`, an immutable structure that contains properties and represents a `Block` at runtime. A `Block` may also define a `TileEntity` to retain continuous data and special interaction logic.
- at runtime every position in the world points to a `BlockState`. This is the object that gets passed in game events and interactions, and may forward block-related events to its representing `Block`.
- relies on the registry system to register and identify each `Block`, on the item system as some blocks may be convertible into their item form, and on the entity system because some logic that `Block` defines is entity-based.
- no evolution standards just yet
- known weaknesses: 
    - *(fixed)* block state registration
    - definition and logic are poorly developed due to lack of features,
    - frontend-backend contract is slightly brittle
    - *(fixed)* deterministic block state properties are missing due to lack of features,
    - no block drops due to lack of features, block to item conversion is not supported
    - underveloped update schedules
    - risk of cycling chain reactions in tick and neighbor updates
    - underdeveloped tile entities logic due to lack of features, state serialization is unavailable.

## Registration
- every `Block` instance **must** be registered to `Registries.BLOCKS`, using one unique identifier. Two instances of the same block that share behavior can be created but must be registered with different ids.
- blocks are registered once at registration time, at bootstrap. Once a block is registered it cannot be removed.

## Block vs BlockState
- data stored in `Block` are characteristics that apply to every `BlockState` that represents it.
- a `Block` must have exactly one default `BlockState`, registered when the block is created. Other states may be registered alongside the default state. Every `BlockState` is immutable after creation. `BlockState`s are registered in a global registry with a numeric ID assigned to each registered state.
- data stored in `BlockState` are immutable and cannot be changed even by the block that created it. Every `BlockState` contains read-only properties defined by the block. A block state is the same as another if and only if they represent the same block and all properties and their assigned values match.
- every valid `BlockState` must be constructible from a finite combination of predefined properties.

## Tile entities
- every `BlockState` may come with a `TileEntity` that `Block` provides through `GetTileEntity(Level, BlockPos)`.
- a `TileEntity` represents an object that can retain information that a `BlockState` is unable to, such as a float that changes every tick.
- a `TileEntity` may also define special interaction logic such as area triggers.
- its important to note that a `TileEntity` is tied to a `BlockState` at a position at all times. If a `BlockState` comes with a tile entity, then the `TileEntity` is destroyed when the state changes.  

## State changes and behavior
- a block state change is triggered through any interaction that affects blocks in the world (player interaction, ticks, neighbor updates, etc).
- state changes are made directly from the `Level`, and currently no validation exists.

## Rendering
- a `Block` declares intent over how a `BlockState` should look like through `GetRenderData` (known issue: brittle block rendering pipeline)
- multiple states can map to the same visual

## Block updates
- blocks currently receive only neighboring state updates, called when a neighboring block has changed state
- a block can be marked as "ticking" if the extended `Block` subtype implements `ITickingBlock` and a state of that block is added to the world. All block states that are mapped to this block will receive a `Tick` call no matter the state, so if there are 2 states of a ticking block in the world, `Tick` will be called 2 times for that block for every tick.

## Known issues:
- *(fixed)* block state registration, definition and logic are poorly developed due to lack of features. Because there arent any features that test the usage, the current implementations are not ideal and too confusing:
    - using a `IBlockStateRegisterer` is unnecessary, as the states need to be registered globally either way. 
    - the default state is awkwardly defined by `protected virtual BlockState GetDefaultState(IBlockStateRegisterer, BlockPropertyEntries)`, when it can be set more easily from the constructor.
    - block states are created through a thin opening, `protected virtual void CreateStates(IBlockStateRegisterer, BlockPropertyEntries)`, then posted to registry all while still unable to access states dynamically.
    - `BlockState` constructor was made internal specifically for `GlobalBlockStateRegisterer`, but nothing stops it from being called.
    - property assignment is too underdeveloped due to lack of features.
    - **this must be fixed before prod**
    - a better implementation would be to switch to a builder pattern, let each state be registered using a state factory which has predefined properties.
- frontend-contract is slightly brittle. A concept that follows "data -> model -> renderer" is used for this contract; `Block` exposes `BlockRenderData` used by `BlockModelResolver` then passed to a `BlockRenderer` used in the world renderer, which works fine for now, but might be changed identifier-based models instead of dynamically retrieved data. For now this is safe to pass to prod.
    - a possible fix consideration would be to switch a pattern like "block state -> model identifier -> renderer", which simplifies the load on the frontend-backend contract.
- *(fixed)* deterministic block state properties are missing due to lack of features. The only blocks that exist at this time are prototypical and use properties only to test the theoretical implementations. Due to the lack of features, there is no way to guarantee that a certain property will always exist on a block state, for example block facing or variant. This doesnt need an immediate fix, but should be considered for prod.
- no block drops due to lack of concrete block usages. **Must be fixed before prod**.
- block to item conversion is not supported. **Must be fixed before prod**.
- underveloped update schedules due to lack of features and need of continuous updates for those features. **Must be fixed before prod**.
- risk of cycling chain reactions in tick and neighbor updates. **Must be fixed before prod**.
- underdeveloped tile entities logic due to lack of features. **Must be fixed before prod**.
- state serialization is unavailable. **Must be fixed before prod**.

## Must be addressed and fixed before prod:
- block state registration, definition and logic
- block drops support
- block to item conversion support
- proper update schedules and flow, cycling chain reactions in updates
- underdeveloped tile entities
- state serialization
