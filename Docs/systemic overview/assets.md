## Top overview
- an asset is a Unity object used for various in-game features
- the asset system is responsible for loading/unloading all assets which and can be retrieved by using a key.
- `AssetManager` is the static class which holds the asset loading/unloading logic. Each loaded asset can be retrieved by using an `AssetKey`.
- at runtime, all assets must first be loaded in order to be used. When its time to load, `AssetManager` first gets a list of all resource locations, then loads all assets at those locations at once *(known issue: no async/streaming model)*
- depends on Unity integration as assets are directly mapped to Unity objects, and on the `Addressables` which is the wrapper for asset management.
- known issues:
    - lacking ownership boundaries
    - no concrete distinction between different asset types
    - no way to prioritize loading/unloading or domain-specific handling
    - fragile asset retrieval
    - no asset state model
    - no async/streaming model
    - no memory management strategy
    - missing dependency tracking
    - weak asset identity

## Asset lifetime
- all assets are loaded at bootstrap. Current used asset types are `Scriptable Object`s, `Tile`s, `SpriteAtlas`es and font assets. *(known issue: no concrete distinction between different asset types)*
- every loadable asset must have the `preload` tag. Any assets that do not have this tag will not be loaded at runtime.
- when its time to load assets, `AssetManager` first gets a list of all resource locations with the tag `preload`. Then it synchronously loads all assets from each location, and assigns an `AssetKey` to each one. *(known issue: fragile asset retrieval)*
- an `AssetKey` is a string-based object used to retrieve assets from `AssetManager`, and contains an address that represents an asset's address. This address must match the one defined in the `Addressables` list.
- at shutdown, all assets are disposed
- note that every asset must be treated as read-only to keep global consistency across assets.

## Known issues:
- lacking ownership boundaries. The `AssetManager` class is static, meaning that assets can be accessed from anywhere, and this means lack of dependency direction. Consider storing an instance of it exclusively in `Soulbound` and only pass the instance to whatever system specifically needs it, such as the UI builders, renderers, audio managers etc.
- no concrete distinction between different asset types. There is no distinction between say a `Tile` asset and a `SpriteAtlas`. All assets are loaded in the same way, but there is no consistency between loaded types. This is also made worse by assigning each asset the same `preload` tag.
- no way to prioritize loading/unloading or domain-specific handling. All assets are currently loade under the hood of the `preloaded` tag. This completely removes domain-specific load/unload and destroys consistency for both asset loading/unloading prioritization and type safety. Additionally, the load order is not deterministic. If any assets have dependencies there is no way to guarantee which one loads first.
- fragile asset retrieval. Theres no type safety between `AssetKey`s. Consider making them generic. On top of this, there is no consistent flow of retrieving assets. Cases like "what hppens if the asset is not loaded?", "is the retrieval guaranteed O(1)?", "is there validation or fallback?" are all disregarded.
- no asset state model. Assets typically exist in states like unloaded, loading, loaded, failed, disposed. `Addressables` provides a way of identifying those states, but there is no support for them, which can make async loading messy later on, systems may request assets in invalid states which will only make debugging more painful. Consider implementing an asset state model before prod.
- no async/streaming model. `AssetManager` loads all assets *synchronously*. This will become a performance bottleneck during prod.
- no memory management strategy. The current implementation loads all assets at once and never unloads them, only at shutdown they all get disposed at once. This was made as a temporary simplification but its missing memory budgeting, unloading policies and scene-based or context-based asset scopes.
- missing dependency tracking. Currently there is no way to tell "who is using an asset right now?". This will completely block whether an asset is safe to unload or what will break if it does unload. Consider adding a reference tracking model
- weak asset identity. Right now an `AssetKey` is used to identify an asset, and it contains an address which must match the addressable asset's address. This is risky because it couples the asset's game identity and Unity's build-time identity. If an asset is renamed or the build pipeline is changed things might break without any warnings. Consider adding a distinction between the logical id (a game-level identifier) and the physical address (addressables key).

## Must be addressed and fixed before prod:
- ownership boundaries
- asset categorization
- loading/unloading semantics and domain-specific handling
- asset retrieval
- async/streaming model
- memory management
- dependency tracking
- asset identity
- `AssetKey` specifications