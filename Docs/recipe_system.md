# Recipe system — documentation

## Overview

The recipe system resolves recipe **assets** into an immutable, queryable snapshot at startup, and exposes a typed query API to the rest of the game.

- **`RecipeAssetResolver`** loads every asset labeled `"recipe"` via `AssetManager`, wraps each into a `RecipeEntry` keyed under the `recipe` registry, and hands the flat list to `ResolvedRecipes.Of(...)`.
- **`ResolvedRecipes`** is the immutable snapshot and contains a dictionary keyed by `RegistryKey<IRecipe>` for direct lookup, and a dictionary keyed by `RecipeType` for type-scoped matching.
- **`RecipeManager`** owns one `ResolvedRecipes` instance and a separate `ingredientIndices` dictionary (from `IRegistryEntryLookup<RecipeIngredientIndex>`), and exposes `Get` / `TryGet` / `GetFirstMatch` / `GetMatching` to callers.
- **`RecipeEntry` / `RecipeEntry<T>`** pair a recipe with its registry key. The generic variant avoids re-casting `IRecipe` to a concrete type at every call site.
- **`IRecipe`** / **`IRecipe<TInput>`** / **`IRecipeInput`** are the contracts a concrete recipe (and its input, e.g. a crafting grid) must implement.

Everything is resolved **eagerly, once**, at construction. There is no per-call re-scanning of assets or lazy resolution. `RecipeManager` and `ResolvedRecipes` are read-only after construction.

- `ResolvedRecipes.GetAll<T>` / `FindMatching` return an **empty sequence** (not an exception) for a `RecipeType` with zero registered recipes.
- `RecipeManager`'s `ingredientIndices` are evaluated (`.GetValue()`) **at construction time**, not lazily on first access. If the underlying registry isn't fully resolved by the time `RecipeManager` is constructed, this will throw or capture stale data at construction rather than at first use.
- The `TryGet*` family relies on `Get*` returning `null` on miss and dereferencing safely via `!` — don't call the `Try*` overloads before the corresponding `Get*` is null-safe if you extend this pattern.
