## Top overview
- the audio system is responsible for playing game sounds, managing temporal and environmental triggers for each audio cue
- its role is to manage sounds that are played based on various triggers, as well as defining the sounds themselves
- the class which handles all audio playing is `AudioManager`. Every sound is represented by an `AudioCue`, a DTO that can be played by `AudioManager`. The sound that an `AudioCue` plays is defined by a `SoundDefinition`. In the case of event-triggered sounds, those can be defined inside a `IAudioEventBank` which can be activated or deactivated based on context or the active domain
- at runtime, every sound can be played at any time, and theres no restriction as to who plays the sound *(known issue: missing sound authority)*
- depends on the event system as sounds and especially sound effects are played on specific events
- aiming to get more dynamic with sounds, as in implement contextualized sounds like ambient music or footsteps
- known weaknesses:
    - missing sound authority
    - `AudioManager` is too centralized and exposes audio capabilities too vastly
    - audio events are poorly designed through audio banks
    - missing concrete audio events due to lack of features
    - audio play policy is not concretely defined
    - missing features from `SoundDefinition` due to lack of features
    - missing settings support
    - audio source pool is poorly designed

## AudioManager
- a static class responsible for playing audio and managing the audio source pool
- sounds can be played by calling `AudioManager.Play(AudioCue)`, where `AudioCue` represents an instance of a sound.
- `AudioCue` is a DTO that contains an `AssetKey` used to retrieve the `SoundDefinition` that will be played. It also contains an optional float for volume override.
- a `SoundDefinition` is a Scriptable Object that represents a sound. It contains a `SoundType` which is one of `SFX`, `UI`, `Ambient` or `Music`, and provides an `AudioClip` from its pool, and has custom volume and pitch support. There is also a possibility to randomize the pitch and the volume inside the `SoundDefinition` inspector, as well as add more `AudioClip`s to the pool in order to randomize which audio gets played. *(known issue: missing features from `SoundDefinition` due to lack of features)*
- every `SoundType` has a default volume and there is a master volume which applies to all sounds *(this is for settings support later on)*. By default, master and all sound type volumes are set to 1f
- every `SoundType` is mapped to a `AudioSourcePool`, an object which retains a collection of `AudioSource`s that can play audio cues. `SoundType.SFX` has a pool of 16 sources, `SoundType.UI` has 8, `SoundType.Ambient` has 4 and `SoundType.Music` has 2. When an audio cue is played, an `AudioSource` is provided by `audioSource.Get()` which will always return the next `AudioSource` in the pool, returning to the source at index 0 if the last source was encountered. *(known issue: audio source pool is poorly designed)*
- note that the pools need be rebuilt by calling `AudioManager.RebuildPools()` when any audio source is destroyed in any pool. This is because `AudioSource`s are cached inside a `AudioSourcePool`, and any calls on destroyed `AudioSource` instances will throw an exception. For example, the pools need to be rebuilt every time the scene switches to or from the world session screen.
- the current audio play policy is audio-independent: play a sound if the retrieved `AudioSource` from the pool is not currently playing any audio *(known issue: audio play policy is not concretely defined)*

## Audio events
- currently the implementation is strictly limited to `EventBus` events, and makes use of `AudioEventListener`s which play a sound when their target event is triggered. *(known issue: audio events are poorly designed through audio banks)*
- `AudioEventListener`s can be added to an `IAudioEventBank` which can be activated or deactivated based on context or the active domain.
- the only audio event banks that exist right now are `UIAudioEventBank` and `WorldAudioEventBank`, which register listeners to their according contexts.
- `UIAudioEventBank` currently only registers a button click event, and `WorldAudioEventBank` registers block placed, block broken and player jump events to audio cues.

## Known issues:
- missing sound authority. There is nothing that tells "who plays an audio right now?" or "which sound is playing right now?". `AudioCue`s are simply published and played on-demand if the sound can be played. This needs to be extended once more features appear before prod.
- `AudioManager` is too centralized and exposes audio capabilities to vastly. Similar to the `EventBus` implementation, the static class `AudioManager` leaks dependency invariants that cannot pass during prod.
- audio events are poorly designed through audio banks. Much like the event system needs a dependency rework, this system also requires a change in how events are dispatched and where the listeners act.
- missing concrete audio events due to lack of features
- audio play policy is not concretely defined. The current implementation creates unresponsiveness when all sources, in the pool are playing.
- missing features from `SoundDefinition` due to lack of features. Volume and pitch randomization are implemented, as well as a scuffed audio clip randomization, but there could be more features added such as clip weights for better randomization.
- missing settings support. `AudioManager` defines MASTER_VOLUME = 1f and a dictionary which maps each `SoundType` to its default value, and there currently is no way to change those. This was made simple for prototyping, and doesnt really need to be changed before prod.
- audio source pool is poorly designed. The current implementation is actually a ring buffer, because it always returns the next source at the next index starting at 0 if it reaches the end. This was made for simplicity reasons, and there are many missing invariants such as source authority or the problem with audio sources being destroyed, non-deterministic playback failures, no prioritization

## Must be addressed and fixed before prod
- sound and audio source authority
- `AudioManager` being too centralized
- audio events design
- audio play policy
- features from `SoundDefinition`
- settings support