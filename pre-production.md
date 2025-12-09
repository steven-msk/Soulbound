# Prototype Phase Summary & Pre-Production Transition

**Date:** November 2025  
**Author:** Miskolczi Stefan (aka seven or MineStef2)  
**Branch:** `prototype`  

---

## Overview

This document summarizes the current state of Soulbound towards the end of the prototype phase and serves as a reference point or a recalibration moment before entering pre-production / complete production.
**All commits following this document contain systems subject to major changes, reworks, and redesign as part of moving toward a production-ready state.**

# Warning
<b>There is an ongoing issue where Zenject fails to compile after copying a Unity project from a ZIP file, even when all files and meta files appear to be present.</b>
The exact cause of this behavior is currently unknown, and there is no reliable fix for now. Avoid transferring Unity projects via ZIP when using Zenject. Use proper folder copy or version control instead.

---

## Prototype Systems & Current Status

 System | Status | Notes |
|--------|--------|-------|
| **Physics** | ⚠️ Planned Refactor | Current implementation feels unnatural, abrupt vertical transitions between blocks not handled correctly. |
| **Audio System** | ❌ Not Implemented | Deferred until production due to backend complexity and timing management concerns. |
| **Tooltips** | ⚙️ Prototype | Currently functional but limited. Future goals: interactable, scrollable, collapsible, and comparable tooltips; support for "legendary" tooltips. |
| **Debug** | ⚙️ Prototype | Subject to redesign for production to handle timing and context sync issues. |
| **Stat System** | ⚙️ Stable, But Rigid | Default stat values are temporary. System may be reworked for flexibility and modularity together with the corresponding dependencies. |
| **World Rendering** | ⚠️ Planned Rework | Current chunk loading causes lag spikes (chunk size: 32x300). Needs optimization and async streaming solution. |
| **Entity System** | ⚠️ Stable, But Unreliable | Subject to a possible redesign or features which require a rework (e.g. LivingEntity in correlation with stats). |
| **Chunk Generation** | ⚠️ Planned Refactor | Required when introducing biomes. Will be tackled during production. |
| **Block System** | ⚙️ Stable, But Rigid | Functional, but slightly limited due to BlockState limitations. Further improvements to be expected in production. |
| **Lighting System** | ⚙️ Planned | Not yet implemented. Core rendering updates must precede lighting integration. |
| **Settings** | ⚙️ Prototype | Functionality stubbed out; will be expanded after UI framework stabilization. |
| **Dependency Injection** | ⚙️ Prototype (Zenject integration) | Currently in a very unstable state. Requires heavy reorganization. |
| **State initialization** | ⚠️ Planned Rework (Zenject integration) | Some systems like the player init still initialize incorrectly due to misleading setup. Similarly to *dependency injection*, this requires heavy reorganization. |
| **Input System** | ⚙️ Stable, But Rigid | Some parts of the implementation are still under the prototype hood, such as the input action request/blocking and event registry. |
| **Internal Timing Communications** | ⚠️ Unstable | Sub-systems, like the Hitbox-Hurtbox relationship, require a better foundation and need to integrate well with events in production (like the upcoming world event system). |
| **Serialization** | ⚙️ Prototype | Most of the data is saved as json, which is memory-heavy and time consuming, at the cost of human readability and maintainability. Will be replaced with a more obfuscated approach later in production. |
| **UI Management** | ⚠️ Stable, but Prototypical | Many of he current features need further improvement and/or refactor. | 
| **Action Request System** | ⚙️ Prototype | Current implementation lives in InputHandler, and is not respecting separation of concerns. Multiple organization related problems are raised alongside. |

---

## Planned Production Tasks

### Feature Implementaions
- Sound effects and music
- Lighting
- Tooltip features like screen clamping and "legendary" tooltips

### Refactors / Reworks
- Physics and movement system overhaul
- World rendering
- Chunk generation logic
- Debug
- UI navigation and management

### Planned QOL / Visuals
- Enhance animation system
- Visual polish and feature binding
- Improve debug and UI consistency

### Notes Related to Upcoming Features
- During production the visuals will be heavily developed, thus a stable animation data and transitioning system is a must before diving into any part of this territory.
- Expect external libraries to appear during this late-prototyping phase and mocked to prepare for production development.

---

## Systems Considered Stable Enough to Carry Forward

- UI communication and screen navigation
- Inventory and item systems, including reverse dependencies like the stat system
- World serialization, saving, and structure loading
- Block system and BlockState implementation
- Testing infrastructure and framework
- Block/Chunk data structures and core logic
- Input system actions including requests/blocks
- Dependency injection and state initialization (despite its messy aspects)

---

## System Dependency Map

- Jump to [Recommended Production Order](#recommended-production-order-top-to-bottom)

Below is each system with:
- General status summary
- Direct dependencies (systems it *needs* to function)
- Reverse Dependencies (systems that depend on it)
- Notes relevant for production

## 1. State Initialization
Status: ❌ Very Unstable
- Bootstraps the entire game state, systems, player, contexts, etc.

#### Dependencies
- Dependency injection (needs a proper DI structure to initialize into)
- Serialization (when loading/reloading)
- Internal timing (event loop ordering)

#### Reverse Dependencies
- *ALL* systems in the game rely on correct initialization order  

> This must be one of the first systems corrected. Almost everything else in prodution depends on proper initialization.  


## 2. Dependency Injection (DI)
Status: ⚠️ Messy, inconsistent
- Provides systems with contexts thet need, prevents circular references, defines system ownership/communication.

#### Dependencies
- None: DI should be foundational

#### Reverse Dependencies
- State Initialization
- UI management
- Physics
- Entities
- World rendering
- Chunk generation
- Input action and contexts
- Audio
- Stat system
- Timing, event callbacks

> Since this is the foundation of maintainable production code, all refactor should assume a stable DI layer.

## 3. Internal Timing / Event Communication
Status: ❌ Not implemented
- Drives hit/hurtbox checking, audio timing, animation sync, particle timing, event sequencing and callback binding, etc. Basically everything timing and communication related.

#### Dependencies
- DI
- State initialization

#### Reverse Dependencies
- Audio system
- Combat systems (hit/hurtbox timing)
- Entities
- Animations
- Input buffering
- Physics timing events
- UI animations (like transitions)

> This is NOT optional. It becomes a multi-system dependency very quickly. Should exist before tackling physics, audio, entities or any advanced system which requires deep event callbacks.


## 4. Input System
Status: ⚠️ Stable, but flawed (blocking, prioritization, event registry issues)
- Central action dispatcher for gameplay, UI, debug, and editor-like features

#### Dependencies
- Internal timing (partially; not every input event needs to be prioritized)
- DI
- State initialization

#### Reverse Dependencies
- Player movement (physics)
- Combat
- UI navigation
- Debug overlays

> Input -> Timing -> Everything else. This must be stable before working on physics or UI.

## 5. Stat System
Status: ⚠️ Rigid, but communication between systems weak
- Governs entity stats, item scaling, upcoming soul mechanics, combat math

#### Dependencies
- DI
- State initialization
- Serialization (eventually; mainly for persistent states)
- Entity system (bidirectional; any entity can eventually acquire stats)

#### Reverse Dependencies
- Entity system
- Items
- Combat
- UI (specifically tooltips, stat visuals)
- Progression systems

> Needs messages between entities, items. Should be stabilized before deep Entity work.

## 6. Entity System
Status: ⚠️ Stable but unreliable
- Manages all entities in the world

#### Dependencies
- Stat system
- Input system (mainly for the player)
- DI
- State initialization
- Internal timing (AI and combat events)
- Block system (collision/correction)

#### Reverse Dependencies
- Physics
- Combat
- Chunk visibility, loading/unloading
- Debug visuals

> Must not be touched until stats, timing, DI and state init are completely stable

## 7. Physics System
Status: ❌ Very unstable, major refactor needed
- Everything related to the physics of the game

#### Dependencies
- Entity system
- Block system
- Input system (for the player)
- Internal timing
- State initialiation
- Stat system (optional, but affects every entity)

#### Reverse Dependencies
- Movement
- Combat hit/hurtbox positioning and hit registering
- AI movement
- Player feel
- Animations

> Touches too many systems; should only be refactored after those systems stabilize

## 8. Block System
Status: ⚙️ Stable but rigid
- Block states, block interactions, properties, block state behavior, etc.

#### Dependencies
- Serialization
- State initialization

#### Reverse Dependencies
- Chunk generation
- World rendering
- Physics (mainly for collision)
- Entity system (for interactions and AI path recognitions)

> The data structures required (i.e. BlockPos, ChunkBlockPos, etc.) need to be completely stabilized. Proceed after you know that these are in a production-ready state.

## 9. Chunk Generation
Status: ⚙️ Stable, but designed for prototyping
- Generates blocks in chunk format; will later support biomes.

#### Dependencies
- Block system
- DI
- State initialization

#### Reverse Dependencies
- World rendering
- Entity system
- Structures
- Lighting

> Should be refactored before world rendering because rendering needs a clear finalized data model.

## 10. World Rendering
Status: ⚠️ Needs major rework
- Efficient rendering of large worlds, streaming, LOD, culling.

#### Dependencies
- Chunk generation
- Block system
- DI
- Camera systems

#### Reverse Dependencies
- Lighting
- Physics visuals
- Debug visuals
- UI (minimap?)

> Rendering must match the chunk pipeline and entity streaming system.

## 11. Lighting System
Status: ❌ Not implemented
- Light propagation, point lights, sunlight, emissive tiles.

#### Dependencies
- World rendering
- Chunk generation
- Block data (specifically opacity)
- DI

#### Reverse Dependencies
- Visuals
- Entities (light-affecting mostly)
- Future ambience/audio systems

> Rendering must be stable first; this cannot be started early

## 12. Audio System
Status: ❌ Not implemented
- SFX, timing, music mixing, triggers, ambience

#### Dependencies
- Internal timing
- Input system
- Entity system
- DI
- Animation events

#### Reverse Dependencies
- Combat
- Player movement
- UI
- World ambience

> Timing is everything here; the internal timing system must exist first.

## 13. UI Systems (Overall)
Status: ⚙️ Stable but needs refinement
- Navigation, layering, screen stacking.

#### Dependencies
- DI
- Input system
- State initialization
- Timing (for animations)

#### Reverse Dependencies
- Tooltips
- Debug visuals
- All UI related systems

## 14. Tooltips
Status: ⚙️ Prototype
- Advanced item/tooltips with scrolling, comparisons, clamping, specialized styling (aka "legendary")

#### Dependencies
- UI systems
- DI
- State initialization
- Input system

#### Reverse Dependencies
- Stats system
- Containers
- Item system
- Stat system

## 15. Debug visuals
Status: ⚙️ Prototype, dependency injection and variable dat organization issues
- Displays chunk info, coords, structure bounds, variable data, input & event logs

#### Dependencies
- UI
- World rendering
- Entity system
- Timing
- DI

#### Reverse Dependencies
- Dev tooling
- Testing

## 16. Settings UI
Status: ⚙️ Prototype
- Reflect global settings, change visual/behavioral settings

#### Dependencies
- UI
- Serialization
- DI

#### Reverse Dependencies
- Rendering (mainly quality)
- Audio (configurations)
- Input (key mappings)

## 17. Serialization
Status: ⚙️ Prototype (JSON, human-readable, but slow and memory inefficient)
- World saves, entity states, items, settings, block states, etc.

#### Dependencies
- DI
- State initialization

#### Reverse Dependencies
- World and Chunk serialization
- Player progression saves
- Settings
- Entity and Block state persistence

> Will be replaced with a more compact production-readt binary or hybrid approach.

## 18. Action Request System
Status: ⚙️ Prototype (implemented poorly, not respecting separation of concerns)
- Resolves event concurrency under specific contexts

#### Dependencies
- DI
- State initialization

#### Reverse Dependencies
- Internal timing system
- Any action systems that happen concurrently

## Recommended Production Order (Top to bottom)
Based on the dependency graph, this could be an optimal order:
[EDIT]: For convenience during late prototyping, categories have been marked accordingly to ease development: `*` means started but not finished, `<-` marks a focus point, `>|` means ready for production, `>>>` means progressively developed (see [notes](#notes))
1. [Input system](#4-input-system) **>|**
2. [Action Request System](#18-action-request-system) **>|**
3. [Serialization](#17-serialization) (foundation only) * *delayed until all WorldDump fields are stable*
4. [Stat system](#5-stat-system) * *some features might be missing, but most systems are working*
5. [Entity system](#6-entity-system) >| *components wil be handled during productions*
6. [Block system](#8-block-system) >| *TileEntity serialization and lifetime aspects TBD when working on the level system*
7. [Chunk generation](#9-chunk-generation) <-
11. [World rendering](#10-world-rendering)
8. [Physics system](#7-physics-system) (universal overhaul)
9. [UI Core layer & navigation system](#13-ui-systems-overall)
10. [Tooltips](#14-tooltips)
11. [Settings](#16-settings-ui) *
12. [Debug visuals](#15-debug-visuals)
13. [Lighting](#11-lighting-system)
14. [Audio](#12-audio-system)

##### Exceptions

1. [State Initialization](#1-state-initialization) >>>
2. [Dependency injection](#2-dependency-injection-di) >>>
3. [Internal timing system](#3-internal-timing--event-communication) >>>

`State Initialization` and `Dependency Injection` are self-deterministic systems. In practice, they do not form two independent layers of abstraction; their responsibilities blend into eachother. Their behavior is based on establishing a complete and coherent object graph before any logic executes.
Because their responsibilities overlap, they cannot be meaningfully separated into more isolated abstractions. The most accurate level of abstraction for both is the "lifecycle unit", a self-contained intialization domain that declares what it requires, what it provides, and how it bootstraps itself.
This means that `State Initialization` and `Dependency Injection` are not isolated modules being "worked on" inidividually. Instead, they are an evolving backbone of the project. Their development progresses naturally as new lifecycle units are introduced. Each improvement in either system automatically reinforces the other, depending on the role they play.  

`Internal Timing` works similarly to whats stated above, but in the runtime domain. Like DI/State init, it is self-deterministic: each unit declares its context, subscribes to events, and gains/loses contexts in a predictable, controlled manner.
The central abstraction for this is the `ContextHandle`, which is a runtime lifecycle hook for any effect or behavior. Each context may differ in how it is activated, but the mechanism for managing activation and deactivation remains the same.
Because every context can define its own runtime behavior, `Internal Timing` cannot be fully generalized into a universal framework. Instead, it evolves alongside new lifecycle units. 
This system is also marked with a special focus point the same as DI/State init.


## Final Note

By December 2025-early January 2026 the project will (hopefully) be considered in a production-ready state.
Starting from this point and until further notice, **all systems are considered mutable and subject to redesign, or complete overhaul if necessary**, for a production-ready foundation.  
The project is now considered to be in a late-prototype state.
