## Top overview:
- a world is a simulation context keeping hold and managing every block, entity and player state
- its responsible for managing block, entity and player state, wired through game events and specific interactions
- content is stored and managed in a Level, a representation of the world's state. Lifetime is managed by a LevelManager
- in execution it serves as the source of truth and management for entity and block states.
- relies on a rendering pipeline to post updates to frontend
- could be extended to support multi-dimentional worlds, requiring different block and entity management contexts, possibly through different instances
- current weaknesses: 
    - world generation is poorly designed
    - entity and block state lifetime arent rigurous enough
    - features are too generalized
    - many dependencies are split across the wrong boundaries
    - player is created inconsistently
    - no serialization

## Mutation model:
- direct access via Level reference *(structural inconsistency)*
- no central validation
- no ordering guarantees
- potential risk of inconsistent state across the lifetime of a world

Mutating a world means any state change made to any block, entity, or the player:
    - mutating a block means that the BlockState associated to a position in the world has changed. This can be done through `BreakBlock` or `SetBlockState`.
    BlockState instances are immutable by design, and there is no way to mutate a BlockState through a data request like `GetBlockState`.
    - mutating an entity means that the properties associated to an existing instance have changed. 
    Entity instances are mutable and can be mutated through data requests, allowing free access to the properties they define and expose, such as their position or attributes.
    - mutating the player works just like mutating any other entity, but may expose special properties which are only available for the player.

Mutations do not have a validation layer. any system that makes changes to the world should be aware of conventions or pull up interfaces in order to avoid runtime collisions between other systems.
> conventions have not yet been defined

## Entity and block state model:
An entity:
    - is created at runtime via constructor or `EntityDescriptor.Create` (construction depends on entity type), every entity instance must represent exactly one entity type.
    - is registered to the world through `AddEntity`, adding an already existing id throws EntityIdAlreadyExistsException. An entity **must be registered to the world** to be in active state. Creating an instance and not registering it right away could lead to invalid states being accessed and read.
    - updated every frame and game tick, scheduled only by the world, no other update schedulers allowed (including Unity's Update loop)
    - destroyed (disposed) when the entity's existence is no longer valid (depends on entity type). Removing an entity is indirect (requires an id, not the entity instance itself) and immediate, and must be called exactly once through `RemoveEntity`, with a reason. Calling `RemoveEntity` again on an already removed id on the same game tick will throw an EntityAlreadyRemovedException *(not yet implemented)*.
> Entity removal reason will be introduced in prod due to lack of features.

A block:
    - has an immutable `BlockState` representation in the world, defined by the block.
    - is constructed exactly once, stored in a registry.
    - every `BlockState` instance has a numeric id used by the world to store data efficiently.
    - setting a block state at a position in the world is done through `SetBlockState`. The state at that position will be replaced immediately by the new state. Attempting to set a block state at an invalid position will throw an InvalidBlockPosException *(not yet implemented)*. Some cases where a block pos is invalid include the pos is outside the bounds, or the chunk at the position is not generated.
    - any block state may be associated with a `TileEntity` used to store continuous data that a `BlockState` is unable to retain, and is able to interact with the world. Tile entities work similarly to regular entities, but must be registered to the world's upate cycle through `AddTileEntity` and removed through `RemoveBlockEntity`. A `TileEntity`'s lifetime is not managed by the world.

## State streaming model:
*Disclaimer: chunk streaming and world generation are too underdeveloped to make up a full section, but as a vague top-down overview:*
- the world is stored in chunks, a 32x1024 section which represents a view into the generated world.
- chunk streaming is currently handled directly by the world, with external updates called per-frame that update the loaded chunks based on a pivot (the player's position)
- chunks are generated procedurally, and its done by the chunk itself exactly once when the chunk is created.
- generated chunks are stored separately from loaded chunks, rendering is also done separately from chunk streaming.
- entities' existence is stored independently of chunks, but this does raise some issues.

## Execution flow
*Similarly to the [state streaming model](#state-streaming-model), the execution flow isnt fully fleshed out. The current implementation is prototypical and will not persist during production, and due to the lack of features it will remain the way it is:*
- every world tick or frame update, every entity and/or block state with a tile entity gets "ticked", which is an update that repeats with a fixed interval of 0.02s (or 50 tps) across the entire lifetime of the world
- during a tick, every entity and/or block state can execute logic according to its features.
- some observations for the current implementation:
    - the tick call order is not enforced, and there is no way to tell when a tickable target gets ticked compared to other targets.
    - the update loop is based on Unity's update loop, some inconsistencies may start to appear after sequential updates
    - the update calls are too centralized and handled in the wrong boundary; all calls originate from a single loop started in the lifetime manager of the world.

## Data access patterns
The current implementation allows data to be accessed and mutated from any source that has an instance of the world itself.
Note that this will not persist during prod. Data access and mutation will be pulled up in different interfaces and systems will not have direct access to all of the world's methods.

## Boundaries
- Rendering: handled externally by a renderer which is passed to the world.
- Input: also handled externally, but results can be propagated within the world context

## Terminology clarification:
- World (definition): static configuration describing rules, generation, and dimensions. Does not contain simulation contraints
- Level (runtime): active simulation instance containing entities' and blocks' state
- LevelManager: responsible for lifecycle of Level instances (supports multiple dimensions)

## Known issues:
- `TileEntity` lifetime inconsistency:
    - tile entities' lifetime isnt managed by the world itself, but they are supported by `BlockState`s which may come with tile entities.
    - possible fix: tile entities that come paired with block states should be destroyed when the the tile entity associated to the block state is changed.
- chunk generation is poorly designed (delayed due to lack of features)
- many dependencies are split across the wrong boundaries (LevelManager constructs both WorldRenderer and Level, even tho its not its responsibility to do so)
- world bootstrapping flow is scattered across multiple types instead of a designated place to kick off from (Soulbound initiates, LevelManager takes constructing responsibility, Level propagates chunk generation but then WorldChunk places the actual blocks, entities arent even bootstrapped); this makes the current flow extremely inconsistent
    - a better flow would be:
        world bootstrap -> block placement (generation if its the case) -> player is created and spawned (block placement is guaranteed to finish before any entity is created) -> all other entities are created and spawned -> update loop starts -> level runtime kicks off from here
- player is created inconsistently; its created by LevelManager when the world session starts, because Player requires a Level instance, but theres no guarantee that the world is generated when the player spawns in. 
- loading a world currently implies switching to a different Unity Scene altogether (a direct SceneManager.LoadSceneAsync call from Soulbound). A special root object (IWorldSceneRoot) is retrieved from the object hierarchy and is used solely for setting up world rendering. This might not be the best approach for the Unity side of world bootstrapping as it raises a lot of issues with cross-scene game objects, transitions both to and from the world scene require a new rendering context every time, which has heavy impact on frontent design. The only advantage here would be separation of concerns regarding world session objects vs non world session objects, potentially helping with better cleanup when transitioning to and from a world, but this may be invalidated by the fact that Unity is only used as the renderer and all objects are to be created manually either way.
- world serialization is unavailable

# Must be addressed and fixed before prod:
- `TileEntity` lifetime inconsistency
- scattered Level and LevelManager dependencies
- inconsistent world bootstrap flow
- inconsistent world update loop design (not too much accent, just the overall flow)
- Unity-side world loading and context
- world serialization

## Some doable entry points for pre-prod:
- introduce controlled mutation and view interfaces (removes direct Level access)
- centralize update loop and enfore execution order
- stabilize entity and tile entity lifecycles
- refactor world bootstrap flow into a single entry point
- decouple Unity scene loading from world lifecycle and bootstrapping procedures