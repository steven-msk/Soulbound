## Top overview:
- a registry is used to register game objects (not to be confused with Unity Game Objects), such as blocks, items, entity types etc.
- it maps every block to an `Identifier`, allowing enumerations on all objects of a type across entire registries
- every registerable object must be added to a registry using an `Identifier`. A `Registry<T>` retains a `RegistryEntry<T>` instance for each object, and can be accessed through a `RegistryKey<T>` or an `Identifier`.
- at runtime it allows objects to be retrieved dynamically by its `Identifier`, for example in command parsing and entity attributes.
- doesnt have any dependencies as its a fundemental system
- may be extended to support tags, which would be used to target multiple registered objects on top of a registry.
- known weaknesses: 
    - registry lookups are slightly underdeveloped
    - possible inconsistency with registries being mutable after bootstrap

## Terminologies
- a **registry** is an object that holds registry entries, which always contain a string ID and the registered object mapped to that ID. A registry's type parameter indicates the accepted type, and includes subtypes, for example a registry `Registry<Item>` maps `Item` instances. Note that every registry is also a registered value of its own, registered in `Registries.ROOT` with its own id, for example `soulbound:block`, `soulbound:item`.
- a **string ID**, or identifier, or simply ID, is represented by an `Identifier` and is a human-readable string that uniquely identifies an object in a registry. Note that 2 registered objects can share the same identifier but **must** be in separate registries. For example a sword item to `Registry<Item>` with `soulbound:sword`, and the model for that item is registered to `Registry<ItemModel>` with the same identifier `soulbound:sword`.
- **the registered object** (or value) is the object added to the registry. A `Registry<T>` type parameter defines the type of the registered object
- every registered object can also be identifier with a **registry key**, represented by a `RegistryKey<T>` which is a combination of the registry id and the object id. It is recommended to use a `RegistryKey<T>` instead of `Identifier` as the type parameter contains the registered object type.
- a **registry entry**, represented by `RegistryEntry<T>` is an object which retains a value registered in a registry. At runtime this is used mostly to keep a registry key and a registered object packed into one, which is simpler than having to force the registered object to retain its own id or registry key.

## Identity model
- an `Identifier` is a human-readable string which has the format `<namespace>:<path>`. If the namespace and colon are missing, the namespace defaults to `soulbound`. Both the namespace and path **must contain only ASCII lowercase characters (a-z), ASCII digits (0-9), or the characters '_', '.', and '-'**. The path can also contain the standard path operator '/'. Uppercase characters are not allowed.
- identifiers are stable across sessions, but may differ from a version of the game to another.

## Lifetime
- registries are loaded at bootstrap, when the game first loads.
- registries and registered objects cannot be removed once they are registered.
- registries are still mutable after bootstrap, by the means of registering more objects, not removing them.

## Access patterns
- registries are stored statically in `Registries` and accessed globally, for example `Registries.ITEMS` or `Registries.BLOCKS`.
- registering an object should be made with `Registries.Register`, and registries can only be created inside `Registries` with `Registries.Create<T>(RegistryKey<Registry<T>>, Initializer<T>)`
- registered objects can be retrieved from each registry using `Get<T>(Identifier)` or `Get<T>(RegistryKey<T>)`. `RegistryEntry<T>` can also be retrieved in the same way: `GetEntry<T>(Identifier)` or `GetEntry<T>(RegistryKey<T>)`

## Known issues
- registry lookups might be slightly underdeveloped. `Registry<T>` implements `IEnumerable<T>` which allows enumerations over all registered objects, but theres no way to get all identifiers from a registry.
- possible inconsistency with registry mutability after bootstrap. By the time all registering has finished, every object should have at least one use somewhere, whether its in frontend or backend. If an object is registered after bootstrap, states are very likely to break and incorrect results may appear. Consider making registries immutable after bootstrap, possibly by "freezing" them. **This must be fixed before prod.**

## Extension: Tags (planned)
- tags will allow grouping of multiple registry entries under a common id.
- some constraints to keep in mind:
    - tags should be resolved after registry bootstrap
    - tags do not modify registry contents
    - tags are read-only at runtime
    - every registry may retain a list of tags it contains. 
