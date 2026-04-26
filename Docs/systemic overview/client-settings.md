## Top overview
- the client settings is a system that aims to give some customizability regarding the game's performance, cosmetics and other mutational data such audio settings, window settings, language etc.
- its responsible for declaring all the client setting entries, as well as managing their values and their serialization and invoke callbacks accordingly
- `SettingEntry` is the class that represents a single, identifiable setting, and it always contains a default value, a value set and a current value. It also provides a `valueChanged` callback which can be used to update in-game state automatically. All settings are stored and managed by `Settings`, a singleton class created directly by `Soulbound` at bootstrap and is responsible for setting serialization and keeping setting entries. A `ValueSet` is responsible for defining value ranges and provide visuals for a setting, as well as coding and decoding target values for serialization.
    - in the case of keybind settings, those are handled by a `Keybinds` class which is created by `Settings`. This class contains all relevant logic for keybinds and has a tight coupling to `Settings`.
- at runtime, settings are used declare changeable data that systems may use. These systems can add `valueChanged` hooks to settings that they target and change game state based on the setting value
- *(later on it will depend on the serialization design once its more mature and possibly on the registry system)*
- current extension point is to integrate visuals for all the different setting types
- known weaknesses:
    - `Settings` exposes setting entries statically
    - no concrete serialization pipeline
    - setting processing is too hard-coded
    - direct Unity InputSystem API
    - unstable key rebinding pipeline
    - unstable value validation strategy
    - no concrete value change propagation
    - possible overload of `ValueSet`
    - missing conflicting key binds resolution and composite bindings, or multiple bindings for an action
    - wrong UI dependency for `ValueSet`

## Settings class and entries
- `Settings` is a singleton class which encapsulates all the game's setting entries.
- every `SettingEntry` is a strongly-typed setting instance that `Settings` may own (not all setting entries need to be defined in `Settings`; entries that are not defined within `Settings` are managed by whatever system uses them, `Settings` has nothing to do with them, serialization is done separately). A `SettingEntry` is defined with a display name, a human-readable string id, a default value and a value set. It also provides a `valueChanged` event that is invoked when the value changes.
- a `ValueSet` is an object which is responsible for defining the possible values of a setting entry, as well as encode and decode the value from serialization, and provide the widget according to the set type. For example, an `EnumValueSet<T>` takes in an array of possible values, `IsValid(T)` will check if the accepted values contains the given value, `Encode` and `Decode` will transform the enum value to and from a string, and `GetVisual` will provide a visual component for changing the value in UI.
- when the game is first loaded, the settings need to be deserialized in order to keep cross-session consistency. When setting changes have been applied, all the settings are saved (serialized with their new values). The serialized contents are written in a separate `settings.txt` file found in the root game directory.
- the current implementation makes use of a `ISettingProcessor` which defines logic that is executed on all the setting entries *(known issue: setting processing is too hard-coded)*. There are currently 2 use cases: a `SettingReader` and a `SettingWriter` that serialize and deserialize the setting entries.

## Keybinds
- a `KeybindEntry` works similarly to a `SettingEntry`, but it is always of type `KeyControl` and has only one possible value set, that being a `KeyboardValueSet`.
- keybind entries are encapsulated within the `Keybinds` class which is owned by `Settings`.
- keybinds target the input actions that an `InputManager` owns, which map to different actions in-game. These can be changed from the settings menu just like other settings. **Note that these only apply to input actions that have a single hardware source.** *(known issue: no explicit key rebinding pipeline)*
- keybinds are managed by `Keybinds` similar to how `Settings` manages setting entries (load/save through processors), except the processors used are `KeybindWriter` and `KeybindReader`

## Known issues
- `Settings` exposes setting entries statically. The current design allows setting entries to be accessed via `Settings.[setting]`, which isnt necessarily bad but may leak dependencies once systems get more mature. **This needs fixing before prod**
    - consider encapsulating the settings and provide getters instead (possibly make use of properties here)
- no concrete serialization pipeline. The only thing happening as of right now is a plain read/write from a file, which is done internally by `Settings` and doesnt rely on any external config, and has inconsistent assumptions (it assumes the root directory is always `Application.persistentDataPath` and the settings file is always "settings.txt"). *Note that this needs to support the upcoming version control*. **This needs fixing before prod**
    - consider making the file path a dependency and possibly an external serializer to remove `Setting`'s responsibility to define serialization logic.
- setting processing is too hard-coded. All setting entries are currently processed one by one, manually passed to the processor. This works for a small number of settings, but will scale poorly once a lot of settings require processnig or dynamic setting entries are introduced. *This doesnt need an immediate fix, but keep this in mind for prod.*
-  direct Unity InputSystem API. The current key rebinds are mapped directly to `KeyControl`s from `UnityEngine.InputSystem` namespace. It might be worth creating a separate engine implementation for these and break Unity environment assumptions, and process the Unity mapping externally. *This is a cosideration to be made when making Soulbound engine Unity independent, doesnt need to be fixed before prod.*
- unstable key rebinding pipeline. There is a support for key rebinding, but the implementation lacks object authority and a lot of edge cases are disregarded. **This needs to be fixed before prod**
- unstable value validation strategy. There is no rule stating what happens when values a deserialized value is invalid, or theres a missing entry. There need to be some fallback rules like reverting to default, or logging and skipping, or partial recovery if possible (this goes hand in hand with version control).
- no concrete value change propagation. There is no enforcement on when `valueChanged` is called, whether its called when the same value is applied, or if it fires after deserialization.
- possible overload of `ValueSet`. Its currently handling 3 domains: value logic, persistence and presentation. Consider splitting the concerns into multiple abstractions if its the case.
- missing conflicting keybinds resolution and composite bindings, or multiple bindings for an action. Conflicting keybinds and composite bindings need to be addressed before prod, while multiple bindings per action can pass to prod.
- wrong UI dependency for `ValueSet`. This is engine code, UI code shouldnt leak in this area. Consider moving the UI logic out of `ValueSet` and make it part of the UI pipeline.

## Needs to be addressed and fixed before prod
- setting entry exposal
- setting and key entries serialization
- UI integration
- key rebinding
- value validation strategy
- `valueChanged` propagation
- `ValueSet` overload concern
- conflicting keybinds resolution, composite bindings