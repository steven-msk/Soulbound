## Disclaimer
- the current debug inrastructure is an early-stage runtime tooling and control system for dev builds.
- in this context, the command system can be treated as a control, metrics as obersvability, the log console as visibility, and tests as validation tools
- **this is not a finalized system and will evolve as the need for mature debugging is requires**. This system is not production-oriented and its features are iterative and expandable. There is no guarantee that these need to be fully implemented before use, and do not make part of a featured build.

## Top overview
- the debug system provides runtime tooling, control, and diagnostics
- it should exist primarily in development/debug builds
- it must be designed so that its non-invasive to core systems. Nothing written in the debugging layer should interfere with the core features.
- acts as a bridge between engine state and dev interaction
- current extension points include a richer command architecture, and make systems able to have debugging plugged in

## Core features
- the command system, which is a runtime execution interface, allows invoking actions, modifying state, or triggering behaviors in a scoped and predictable manner
    - this system acts as a developer control surface, and can be extended per system *(very important extension point)*.
    - could be useful for example for spawning entities, modifying stats, or forcing states
    - *this is essentially the engine's runtime CLI*
- the metrics system is a data collection layer for runtime values like FPS, memory, entity counts, timings
    - can be polled, displayed or logged
    - it acts as an observation point for data at runtime
- the log console is a centralized output stream for runtime information
    - this aggregates logs from systems and displays info, warnings, errors depending on what happened
    - this can be seen as a temporaly visibility layer, providing a history for what happened on the course of the game's lifetime
    - *side note: this will likely integrate with command input*

## Testing integration
- tests are validation tools, and do not take place at runtime debugging
- **debug systems may expose hooks for tests or reuse logging or metrics**
- tests operate offline through unit tests or in controlled runtime environments

## Prod consideration
- debug features may be disaled in release builds or gated behind flags (might make them available in snapshots, alpha or beta builds)
- performance overhead should be minimal, as in features shouldnt add too much workload on performance
- logging verbosity should be configurable
- it is worth mentioning that, during production, many things can (and probably will) go wrong. As such, here are some future directions for features that could make debugging less painful:
    - debug overlays (scoped visual metrics)
    - entity inspectors
    - command autocomplete / history (currently implemented, but poorly adapted)
    - runnable command scripts; files in which a set of commands are executed sequentially. *This could be useful for in-game tests*
    - runtime toggles (or cheats), like god mode, noclip, night vision etc.
    - event tracing