## Principle
- **Unity is a host runtime service proider, not a part of the engine domain**
- all Unity API logic must terminate at the adapter layer
- this means:
    - no `UnityEngine.*` types cross into `SoulboundEngine`
    - the engine works on pure data and abstract interfaces
    - unity handles presentation, IO, and lifecycle hooks

## Execution and lifecycle integration
- *this is the root integration point*
- Unity provides `MonoBehaviour` lifecycle (`Awake`, `Start`, `Update`, `LateUpdate` etc.), scene loading/unloading
- the engine recognizes deterministic update entry points
- the adapters' responsibility is to map Unity loop to client lifecycle, for example `Update -> client.Tick()`, `LateUpdate -> client.LateTick()`
- design implication: the adapter is authorative entry, not optional

## Rendering integration
1. Camera
    - makes use of Unity's `Camera` component
    - engine abstracts camera state, like position, zoom, bounds etc.
    - adapter syncs engine camera with the Unity camera transform and properties
2. Renderable objects (entities, items, blocks etc.)
    - represented in Unity with `GameObject`s, `Transform`s, `SpriteRenderer`s etc.
    - the engine exposes render state (position, identifiers, animation steps)
    - the adapter maps engine render data to Unity objects
    - *note that the engine doesnt know about the existence of any GameObjects. Unity objects should be treated as projections, not sources of truth*
3. Animations
    - unity integration through `Animator`, `AnimationClip`s or other custom systems
    - the engine owns animation state machines (or future equivalent)
    - the adapter converts engine animation state to Unity playback
4. World rendering
    - unity integration through `Tilemap` or mesh rendering
    - the engine provides chunk/block data
    - the adapter converts block data into visual tiles or meshes
5. Lightning and effects
    - unity handled lights, shaders, post-processing logic
    - the engine abstracts lightning or effect triggers
    - the adapter translats engine events to Unity visual effects
6. Custom rendering pipelines
    - makes use of URP/HDRP hooks and `CommandBuffer`s
    - the adapter should encapsulate pipeline-specific behavior

## Input integration
- Unity side InputSystem which maps hardware input to action triggers
- the engine abstracts input actions and states
- the adapter maps unity input events to engine input events
- *engine never sees keycodes, devices etc.*

## Audio integration
- Unity's `AudioSource` for playback, `AudioClip`s for sound definition
- the engine emits sound events as well as data accompanying them
- the adapter converts the sound events to Unity playback

## Asset management integration
- assets are managed by Unity through `Addressables` library
- the engine contains asset identifiers or references
- the adapter resolves identifiers to Unity assetss
- *note that the engine never holds `UnityEngine.Object`. Unity assets returned by these adapters should be used passed to other adapters that translate Unity assets to engine assets, if the engine explicitly requires them*

## Entity presentation
- Unity mirrors engine `Entities` through `GameObject`s
- the engine exposes pure entity data (ECS or otherwise)
- the adapter synchronizes entity state with Unity objects
- *note that unity object lifecycle must mirror engine state, not lead it*

## Physics integration *(unstable)*
- works with Unity's `Rigidbody2D` and associated 2D colliders
- the engine abstracts physics state or defines a custom system
- *decision point: use Unity physics or isolate logic behind adapter*

## UI integration
- integrated through Unity's `Canvas`, ui toolkits, `EventSystem`
- the engine owns UI state and data
- the adapter maps UI state to Unity UI elements

## Debug and tooling integration
- Unity's console, gizmos or editor tools can be used within this context
- the engine provides debug commands, metrics
- the adapter bridges engine debug to Unity visualization or logging

## Serialization and persistence hooks
- integrates within Unity's asset serialization and scene data
- the engine owns the save and load systems
- the adapter handles Unity-specific assets or object persistence

## Threading and async integration *(unstable)*
- Unity-side main thread restrictions, `UniTask` library and async addressables
- *extension point: the engine may introduce async systems later*
- the adapter ensures Unity APIs are only called on valid threads, and mirroring engine state consistently

## Design implications
- each integration falls into one of these:
    - push from the engine to Unity (rendering, audio, UI)
    - pull from the engine to Unity (input, lifecycle hooks)
    - bidirectonal relation (entities, assets)
- because of how adapters differ:
    - thin adapters are preferred for the input, audio and the lifecycle systems
    - medium-sized adapters are required for the camera and UI
    - complex adapters could be seen for rendering, entities and the asset system
- for design consistency, adapters are allowed to pass primitives, engine-defined structs, identifiers and handles to the engine. **No `GameObject`s, `Transform`s, `ScriptableObject`s, or ANY Unity type is allowed to enter the engine's codebase**

## Determinism concern
- it is important to note that **Unity is not deterministic**, and that the adapter layes must normalize timing, control the update order and prevent Unity-side state from drifting
- **it is the adapters' responsibility to manage and keep Unity state synchronized to engine state.** If anything goes wrong and the problem seems to be from Unity, the first to be checked are the adapters.