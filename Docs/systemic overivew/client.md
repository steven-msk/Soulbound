## Disclaimer
- this doc describes the client as the cetral local execution authority
- *multiplayer is not currently supported, but it is planned for later versions. *The "client" term is made intentionally to anticipate that*
- **even in a singleplayer context, the client behaves as it were a network client with full local authority**

## Top overview
- the cliet is a runtime authority root for all gameplay execution
- it owns and drives all player-facing systems such as input, sound playback, rendering, settings etc.
- it works as a singleton owned by the main engine entry (Soulbound)

## Authority model
- the client holds and operates on the active world context and has authority over its simulation, thus it owns the player entity.
- it also has authority over input, camera behavior, UI interaction, gameplay execution flow etc.
- for each system, the authority it targets depends on specific facilities:
    - for the input system, on 'raw input -> processed actions' resolution
    - for the rendering pipeline, on camera and render dispatch
    - for the audio system, on sound playback and controls
    - for the UI system, on menus, HUDs, overlays etc.
    - in debug systems, on the console, metrics, commands etc.
    - for the settings, on mapping the actual setting values to their uses
- *systems must query the client for context-sensitive decisions*. No system should assume global control without client awareness
- the client also owns the adapter layer, the bridge between Soulbound engine and Unity API. *Note that the client owns only the existence of the adapters, not their implementation, and adapters are driven by the client, not the other way around*. 
    - the adapters it owns include rendering, input and audio as these need a direct Unity runtime hook.
    - adapter implementations are defined in the engine integration layer

## Player entity
- the player is a controlled entity indirectly driven by the client, i.e the player doesnt interpret input directly, the client does.
- the client's role is to bridge between user intent and player behavior, and to respond to events accordingly

## Singleton clarification
- the client is a global static singleton created by the core engine's entry (Soulbound).
- there is always one and only one active client for the entire duration of the game.
- all local systems require shared synchronized access, never reference gameplay systems by crossing the client
- note that it is made global mainly to avoid unnecessary dependency crossing across systems. It is encouraged to pass the instance to deeper dependency graph nodes, and let higher-level systems grab the instance via `SoulboundClient.instance`

## Lifecycle
- during its lifecycle, its responsible for updating the game every frame and keeping the game's state consistent with the engine as well as Unity API *(known issue: undefined update sequence)*
- at initialization, it must build all the necessary objects and/or adapters that may be encountered on the course of the game
- at shutdown, it is also responsible for closing all instances deterministically
- to get a better understanding of the update sequence, a rough update flow would be input -> simulation -> rendering