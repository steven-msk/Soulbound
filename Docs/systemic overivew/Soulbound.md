## Disclaimer
- this document doesnt present a system by itself. It presents an overview of what the Soulbound class is responsible for, what it owns, and prod considerations
- since this is the Soulbound engine's core, **all the issues mentioned below should be acknowledged and devs (including me) should be cautious with decision making**

## Soulbound class
- the `Soulbound` class is the highest-level type where the game's core engine context lives
- `Soulbound.instance` is a global static singleton and only one instance can exist at a time. 
- when the game starts, it must create an instance of `Soulbound` and call `Soulbound.Launch`, kicking off the engine runtime environment. Once `Launch` has been called it cannot be called again.
- the `Soulbound` constructor takes in a `GameConfig` instance which is used to set up game state. The is immutable config data and spans across the entire lifetime of the game.
- while the `Soulbound` constructor is running, the game's phase is considered to be `bootstrapping`. After `Launch` has been called the game's phase switches to `launched`. *(this is not yet concretely implemented but should exist in prod)*

## Soulbound's role
- its main purpose is to set up global game state and bootstrap the engine in a completely deterministic order
- there isnt a concrete order of initialization yet, because of how many inconsistencies there are with the current implementations *(this will probably be fixed as systems get more mature and require deeper depedency graphs)*
- but most importantly, its responsible for keeping the game's state intact across its lifetime and assuring that the engine is alive. As long as the application is running, the `Soulbound` instance should never dissappear.
- another important role is to bootstrap the actual Soulbound client, and initiating all relevant systems such as assets, resources, input, UI, audio, serialization and rendering pipelines, client settings, lifecycle hooks, update scheduling setup, dev and debug aspects etc.

## Current design flaws
- Unity API leaks and unsafe usage
    - for example the unsafe assertion that the Unity scene is already loaded during bootstrap (`UIHandler` takes in a `Canvas` which is passed with `UnityEngine.Object.FindFirstObjectByType<Canvas>()`), same as the `AudioManager` pools being rebuilt (creates `GameObject`s with `AudioSource` components)
- there is no concrete order of initialization. Everything that is currently being initialized is done in a "first in, first served" manner (except for systems that explicitly depend on other systems, in which case the initialization order follows what system has which dependency)
- poor dependency graph initialization and object authority (client-specific objects are leaked from bootstrap)
    - consider introducing either a topological initialization graph or a staged initialization phase
- lifecycle hooks are poorly implemented (the current `SoulboundUpdateScheduler` being managed exclusively by Unity and injects updates directly in `Soulbound` without any contract), Unity lifecycle hooks are disregarded
- no explicit game phases
- `Launch` has no authority over the (nonexistent) game launch phase. Currently it only locks the running state, sets the name of the current thread to LaunchThread, adds a hook to Application.quitting event, and sets the screen to the title screen.
- client features leak in engine context. `EnterWorld(string)` and `QuitActiveWorld` are defined within `Soulbound`, but these should be client-specific methods.
- **missing threaded execution concerns - this will be heavy on prod**

## Engine and prod considerations
- the `Soulbound` class needs to have its object authority split up. There should be another class `SoulboundClient` which would ideally be responsible for the engine's client side. This class should be instantiated directly by `Soulbound` during bootstrap and should be treated as a dependency graph node for other systems.
    - this where `EnterWorld` and `QuitActiveWorld` as well as the input and settings pipeline would be moved to. 
- `Soulbound` should only remain with the core engine systems like the registry, assets and resources, serialization, debug, engine-level config etc.
- an explicit lifecycle model based on a state machine should exist. A tyipical flow would be:
    - uninitialized -> bootstrapping -> initialized -> launching -> running -> shutdown -> terminated
    - each transition should be atomic, validated (as in no re-entry, no invalid jumps) and emit lifecycle events or hooks
    - **each state must be formalized and explicitly enforced by the engine**
- formalize object creation and ownership. `Soulbound` should own engine singletons, `SoulboundClient` should own client systems, and systems cannot outlive their owner.
- **do not disregard threading concerns**. There may be a lot of problems regarding thread-safety, especially with static singletons. These need to be addressed before the versioning system is introduced.
- enforce the boundary between Soulbound engine and Unity. Unity should be the host, Soulbound should be runtime authority, and the update loop must be owned by Soulbound, executed through Unity, **not injected by Unity**. There needs to be a concrete Unity abstraction boundary, where Soulbound engine and Unity API are separated through Unity adapters (or a platform layer)
- an error handling strategy needs to be defined at root level. This means a fail-fast or recoverable strategy, structured logging and potentially state rollback in the case of fatal errors happening in important game phases like bootstrap.
- for soon-to-come versioning and compatibility, `Soulbound` should be the natural anchor for version control. It will become tied to the save system compatibility, config versioning and migration logic.
- *note that most of these considerations will be be decisions that are made and implemnted with time, as systems get more mature, not necessarily as one-time final decisions*
