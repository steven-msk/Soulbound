## Disclaimer
- much like UI, this document is a high-level overview and only presents possible implementations, and aimed towards clarifying the current and future designs

## Current implementation
- the current implementation is split into 2 divisions: world rendering and UI rendering.
    - world rendering follows a rendering resolution pattern, as in backend exposes render data based on state, and renderers consume it to output visuals. Eventually, the pattern resolves to Unity API which is the code found in renderers.
    - UI rendering makes use of builders that build the UI elements, as well as mapping input to behavior. There isnt a concrete "rendering" pipeline as UI elements are created once and their render states are managed internally by Unity.
- *be aware that this implementation is not fundementally wrong, but the design clashes with the backend-frontend contract, and Unity API is poorly injected into this context. This is something that needs fixing ASAP, before prod. See [target system](#target-rendering-system-design) below*

## Target rendering system design
- the rendering system refers to the system that translates visual intent to unity execution
- the rendering model is engine-driven (decoupled from Unity). This means that the objects used in this system are not tied at all to Unity's API, but eventually resolve to it through domain renderers.
- the atomic unit that the game produces is represented by a renderable object, which refers to the state of an object that appears on the screen (Unity-side realization)
    - this usually implies a render state object, a data object which contains information about the rendered model, things like transform, object characteristics etc
    - a model is the object that defines the geometric shape of a renderable object.
    - a renderer is an object that resolves a render state and produces visual output. Note that not all renderers require a model, some renderers may only render features that dont necessarily require a specific geometric shape, such as an area of effect.
- on top of all renderers which target specific objects there is a domain renderer which aims to control the lifecycle of the rendered objects. This is the core renderer for the domain where the rendered objects are mapped to Unity API and manages the render order.
    - domain rendering also takes responsibility for detecting render state changes through diffing between consecutive frames
    - if it detects a render state being added, it creates a rendered object for it and assigns the numeric id mapping to Unity API
    - if it detects a render state being removed it destroys the object and removes the id
    - if it detects a render state being changed (its data got mutated) it updates the rendered object accordingly.

## Frame rendering model
- rendering operates on a per-frame submission model
- each frame, the game produces a complete set of render states representing the current visual output, which are collected in a `RenderFrame`
- the domain renderer consumes the full frame and resolves it to Unity API

## Model definition
- a model represents shared, reusable visual data such as the mesh
- models are asset-level constructs, not per-instance data
- render states reference models but do not own them. Some renderer implementations may not even require models.

## Backend contract
- backend systems are responsible for extracting render states from game state.
- rendering systems are allowed to query backend data and create render states by themselves, but only in a limited scope, preferably inside an object renderer where it can grab the data directly from the backend object.

## UI rendering integration
- there are some hard rules which need to be followed for the target design to work:
    - UI elements never directly interact with Unity rendering
    - UI produces render states just like world systems
    - UI and world share the same rendering pipeline, but focused on different domains
- note that the implementation must support an input design

## Rendered object identity
- each rendered object must have a stable numeric identifier across frames which is used by a domain renderer to map to Unity objects
    - identfiers must remain stable for the lifetime of a visual object
    - identifiers must be unique within a render domain
    - changing an identifier is treated as destroying and recreating the rendered object
    - identity generation is the domain renderer's responsibility
- a rendered object originates from a render state and essentially represents it visually. Therefore, a render state is an open DTO that can be reused.
- a render state should be a frame-scoped data representation, so treat them as immutable during a frame. Reuse across frames is allowed but must be done carefully to minimize shared mutable state side effects.

## Render layers
- render layers define a deterministic order between domain renderers
- there are currently 3 render layers: world, UI and overlays
- *these arent defined concretely just yet, and need a different topic altogether which needs to be discussed before prod*
    - a typical consideration would be to define deterministic ordering within layers, so ordering between layers is strictly enforced

## Transform clarification
- the `Transform` that a render state exposes is not the same as Unity's transform
- this transform is used to give spatial context to a render state, so a position, rotation and a scale.
- the target implementation should be close to Unity's Transform design
- transforms may be hierarchical, in a tree-like structure. Child transforms are always relative to the parent and the renderer resolves the final shown transform.