# Soulbound

---

- Soulbound is a 2D sandbox survival game built with Unity, inspired by Terraria, Hollow Knight, and Minecraft.

**Current Stage:** Alpha Production

---

### Table of Contents

- [Overview](#overview)
- [Features](#features)
	- [World generation](#world-generation)
	- [Block system](#block-system)
	- [Entity system](#entity-system)
	- [Inventory & items](#inventory--items)
	- [Player](#player)
	- [UI framework](#ui-framework)
	- [Settings & keybinds](#settings--keybinds)
- [Getting started](#getting-started)
	- [Requirements](#requirements)
	- [Running the game](#running-the-game)
	- [Versioning and branching](#versioning-and-branching)
	- [Workflows](#workflows)
- [Architecture](#architecture)
	- [Assembly](#assembly)
	- [Tech stack](#tech-stack)
	- [Debug & dev tools](#debug--developer-tools)
- [Known issues](#known-issues)
- [License](#license)
- [Notes](#notes)

---

## Overview

Soulbound is a 2D survival game currently in alpha production. The core engine systems are being built with a strong emphasis on clean separation of concerns, modularity, and production-readiness. The game features procedurally generated worlds, a block-based environment, entity interactions, and a fully functional inventory system.

---

## Features
> **Note:** These are implemented by an alpha production standard and may be reworked with further iterations.

#### World Generation
- Procedural terrain generation
- Biome blending using weighted noise sampling
- Cave system generation with domain warping and vertical falloff masks
- Chunk-based world loading and unloading based on player proximity
- Post-processing passes per biome (e.g. tree placement)

#### Block System
- Block states with arbitrary property entries (int, bool, enum)
- Tile entities
- Neighbor update propagation
- Interaction listeners
- Ticking blocks

#### Entity System
- Entity registry using descriptor-based factory pattern
- Physics entity with Unity Rigidbody2D integration
- Item entity (dropped Item form)
- Frame-updatable and tickable entity interfaces

#### Inventory & Items
- Slot-based inventory and hotbar system
- Drag-and-drop with split, merge, transfer, and swap operations
- Item interaction listeners with trigger validation (click, hold, release)

#### Player
- Movement, jumping, and physics-based traversal
- Block breaking and placement with reach limitation
- Item drop mechanic
- Hotbar scrolling and main hand switching
- Input handled through a priority-based context stack

#### UI Framework
- Screen management with push/pop/replace navigation
- Overlay system for non-screen UI (debug console, command line, metrics HUD)
- Tooltip design with trigger and renderer interfaces
- Layout builders for vertical, horizontal, and grid arrangements

#### Settings & Keybinds
- Settings serialized to a plain-text file
- Per-entry encode/decode via value sets
- Keybind rebinding with input action override *(runtime rebinding available only through code)*

---

## Getting Started

### Requirements

- Unity 6000.3.14f1 (6.3 LTS) or compatible version
- .NET Standard 2.1

### Running the Game

1. Clone the repository:
   ```bash
   git clone https://github.com/steven-msk/Soulbound.git
   ```
2. Open the project in Unity.
3. Open the `DevScene` scene at `Assets/Game/Scenes/`
4. Optionally configure `GameConfig` on the `Main` game object in the scene.
5. Press Play.

### Versioning and branching
- `main` as primary branch (active development)
- Feature work may be done in separate branches and merged when stable
- *No strict versioning is enforced at this stage*

### Workflows
-  *Deferred for a later release*

---

## Architecture

Soulbound is designed around a backend-first architecture. Unity-specific concerns are isolated an adapter layer.

### Assembly

All game code lives under the `SoulboundEngine` assembly definition (`SoulboundEngine.asmdef`).


### Tech Stack

| Technology | Purpose |
|---|---|
| Unity (2D) | game engine and rendering |
| C# | primary language |
| UniTask | async usage (no Unity coroutine overhead) |
| Newtonsoft.Json | serialization |
| FastNoiseLite | procedural noise |
| Unity Addressables | asset management |
| Unity Input System | input action management |

---

### Debug & Developer Tools

Soulbound ships with a suite of in-editor and in-game developer tools.

#### Command Line (`/`)
Press `/` in-game to open the command line. Supports:
- Tab completion
- Command history cycling with up/down arrow keys
- Available commands: `tp`, `setblock`, `spawn`, `give` *(only inside a world)*

#### Debug Console (`F1`)
Displays all Unity log output (Info, Warning, Error, Exception) with per-type filtering.

#### Metrics HUD (`F2`)
Displays real-time performance data:
- FPS and frame time
- Fixed update time
- Managed memory, mono heap
- GPU memory
- GC allocations per frame

---

## Known Issues

These are intentional deferrals, not bugs:

- **Serialization** - World save/load is partially stubbed. Full NBT-style serialization is deferred to a later milestone (beta or later).
- **Lighting** - No lighting system yet. Deferred until the rendering pipeline stabilizes.
- **Animations** - infrastructure exists but visual animations are not yet wired to gameplay.
- **Modding Support** - Systems are designed with modding in mind, but modding APIs are explicitly deferred.
- **Visual Polish** - UI visuals, shaders, and artistic assets are placeholder or absent. Production art is out of scope for alpha.

## License
This project is licensed under the MIT License. See [LICENSE](./LICENSE) for more details.

## Notes
- This repo is in active development of the game. Features, systems, and structures are subject to change as development continues.