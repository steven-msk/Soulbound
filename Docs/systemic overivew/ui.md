## Disclaimer
- this document is a high-level overview of the UI system altogether, and is not aimed towards clarifying the current implementation structure
- this is due to the highly inconsistent and risk of premature system locking

## Top overview
- the UI system manages screen-based UI composition and interaction
- screens act as isolated UI contexts
- UI is interpreted as a hierarchical tree of elements
- interactions are event-driven (mostly)

## Current stable points
- screen navigation uses a stack model
- UI is constructed at runtime (no editor-driven layout)
- elements are hierarchical, in a tree-like structure
- tooltips are trigger-driven, not always present
- UI mutation happens through handles *(this might point to a design flaw)*

## Known design flaws
- unclear ownership of UI element state vs. handles
- builder pattern may be overengineered for current scope
- layout system is underdefined, frames and layouts are vague abstractions
- no formal event propagation model, unclear capturing and bubbling
- unclear coupling to Unity rendering layer
- lack of declarative UI description

