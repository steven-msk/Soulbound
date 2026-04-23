## Disclaimer
- this document isnt a systemic overview by itself. It provides information about the camera object and extension points for alpha prod.

## Top overview
- the camera is a client-driven device through which the player views the world
- its responsible for the lowest phase of game rendering, where all rendering systems eventually converge into.
- Unity API provides the `Camera` component to access camera data and apply special rendering logic. *(Note that the Camera object itself will not be exposed directly to Soulbound engine)*
- at runtime, the rendering itself is managed by Unity, but the camera control and lifecycle is hooked into the engine through an adapter. Camera control features such as shakes, targeted focuses etc. are managed by Soulbound engine and translated to Unity API through an adapter. Lifecycle hooks depend on the game's state (different callbacks may be hooked up if the camera is in a world vs. the title screen).
- will depend on Unity integration and adapter system as the technical camera logic is driven by Unity API.
- current extension points include introducing targeted focus through a camera ownership model and camera motion. There should also be integration with the settings, for example a field of view control, parallax layers integration
- known weaknesses:
    - no camera ownership
    - missing camera-player correlation
    - prototypical player focus implementation
    - missing explicit lifecycle hooks

## Coordinate space and projection
- the camera is always 2D ortographic, so no perspective
- the unit scale is 1 Unity unit = 1 block (tile)
- works with world-space coordinates and translates them to screen-space coordinates through ortographic projection
- zoom is abstracted as a scalar value mapped to Unity's ortographic size 

## State model
- there are 4 different states that the camera can be in:
    - idle, when the camera is not doing anything
    - following a target, for example the player
    - transitioning, for example lerping from one target to another
    - scripted, when forced control is applied
- states are managed by a state machine, which every frame queries the current state and applies relevant logic. If the current state is scripted it does not apply any logic (simulates idle, but logic should be managed by whoever set the camera's state to scripted. *Note that the scripted state must be set with a key and it can only be reverted with that key*).
- an example flow would be:
    - target updates pos -> camera resolves desired pos -> apply optional transitions (e.g smoothing) -> apply optional effects (e.g shakes, offsets) -> push to Unity Camera

## Motion model
- a camera target is any object that the camera will focus on, update its relative position and point its state transitions towards
- there are 4 general tracking modes:
    - hard lock, where the camera updates its position to closely match the target's position
    - smoothed follow, where smooth lerping is applied to the camera's position changes
    - dead-zone tracking which makes use of regions or exit windows to smooth out target vs. camera motion
    - look-ahead tracking which makes the camera's position be biased based on the target's velocity
- camera motion is configurable and allows interpolation types like linear, smooth damp or spring-based, with configurable parameters such as damping or responsiveness.
- camera effects include shakes, offsets, impulses and any other temporary changes made to the camera at some point in time. These can be duration-based or triggered by events. Effect composition is not directly supported, as in multiple effects can happen at the same time, but this is not managed in any specific way. *(this might cause some problems in prod, but can postponed to a later time)*
- camera position may be clamped within world bounds or dynamically constrained by custom volumes.

## Unity contract
- the engine camera talks to Unity's Camera through an adapter layer
- it sends data like position, rotation, zoom, fov etc. to Unity's API
- the adapter layer **mustnt** expose Unity Camera internals
- the adapter should serve only as a translator and doesnt execute any logic by itself

## Settings integration
- a few settings can be defined that can be passed to the adapter:
- these include zoom sensitivity, camera smoothing toggle, shake sensitivity, and other accessibility options like motion reduction.

## Performance considerations:
- the camera adapter and especially motion handling will be heavy on performance.
- consider making as little allocations as possible because the adapter will be called every frame, deterministic updates and no heavy math in hot paths