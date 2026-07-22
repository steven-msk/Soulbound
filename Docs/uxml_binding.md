# UXML Binding System


Every named element in a `.uxml` file was previously accessed via `VisualElement.Q<T>("SomeName")`, with the string typed by hand at every call site. Nothing checked that the string matched what was actually in the UXML, nothing checked the type you cast to was right, and nothing warned you when a rename on either side (UXML or C#) silently broke the other. This system replaces that with a build-time-generated schema and a typed runtime accessor, so a mismatch fails loudly, either at codegen time or the first time you call `Get<T>`, instead of returning `null` deep into a play session.


**The schema (`UXMLSchema`)** is a static, frozen `Dictionary<Identifier, Type>` built once at startup. It answers *"what type does this identifier resolve to?"*. It never holds an actual `VisualElement` instance, because nothing is instantiated yet.

**The runtime accessor (`UXMLBindings.Get<T>`)** is an extension method you call against an *already-instantiated* root `VisualElement` (e.g. the result of a `CloneTree()`). It looks up the identifier's expected type in the schema, validates it against the `T` you asked for, then does the actual `Q<T>()` call underneath.

There is no global table of live elements. Multiple screens, or the same screen reopened, each produce their own independent instance tree, so resolution always happens against whatever root you hand in.

---
##### The `Identifier` format

VisualElements inside a UXML file are be identified in code by following the Identifier format:

```
[namespace]:[document_id]/[element_name]
```

- **namespace**: `soulbound` for anything shipped in the base game. Reserved for mod namespaces in the future (see below).
- **document_id**: the snake_case form of the `.uxml` file's name.
- **element_name**: the exact `name` attribute from the UXML.

For example: a `Label` named `FPS` inside `MetricsHUD.uxml` becomes:

```
soulbound:metrics_hud/fps
```
---

##### How the schema is built

A `UXMLSchemaGenerator` (editor-only `AssetPostprocessor`) scans every `.uxml` file under the configured project roots whenever one is imported, modified, or deleted, and regenerates a single file `UXMLSchema_Generated.cs`. You can also trigger this manually via **Soulbound -> Regenerate UXML Schema**.

For each named element it finds, the generator resolves the UXML tag to a CLR type:

| UXML tag | Resolves to |
|---|---|
| `ui:Button`, `ui:Label`, etc. | The matching built-in UI Toolkit type (`Button`, `Label` etc.) |
| A custom tag (e.g. a project-defined element) | Found via reflection, if a matching `VisualElement` subclass exists |
| `ui:Instance` | `TemplateContainer`, the wrapper node that appears in the instantiated tree where a template is used |
| `ui:Template`, `ui:Style` | **Not registered.** These are declarations, not nodes; they never appear in an instantiated tree, so there's nothing to look up |

The generated file is committed alongside the rest of the codebase and gets rewritten in full on every regeneration. Never edit this file by hand.

---

##### Using in code

You can use this system to identify `VisualElements` from code:

```csharp
private static readonly Identifier HOTBAR_ELEMENT = Identifier.Of("soulbound:hotbar/hotbar");

protected override void OnBind(VisualElement root) {
    VisualElement hotbar = root.Get<VisualElement>(HOTBAR_ELEMENT);
    // ...
}
```

`Get<T>` throws `UXMLBindingException` in two cases:
- the identifier isn't in the schema at all (typo, or the element was renamed/removed in UXML without regenerating), or
- the identifier resolves to a type that isn't assignable to the `T` you requested (e.g. asking for `Button` when the schema says `Label`).

Either way, you get a specific, actionable message instead of a silent `null`.

---

##### Element names must be valid identifiers

Not every string that's legal as a UXML `name` attribute is a valid `Identifier` element name. If a name doesn't satisfy `Identifier.IsValid`, the generator skips it, a `[UXMLSchemaGenerator]` warning will appear in the console, and it won't appear in the generated schema.

This doesn't break anything at the UXML level, the element still exists in the tree and `root.Q<T>("ThatName")` still finds it exactly as before. However it is recommended to use this identifying system for type validations and less ambiguity between elements.

---
##### Elements inside templates use the template's own identifier, not the parent's

When a screen's UXML references another document via `<ui:Instance>`, the elements *inside* that referenced document keep their own document's identifier, they are not renamed to live under the parent's namespace.

Concretely, `WorldScreen.uxml` has an `Instance` named `CommandLine`, which resolves to:

```
soulbound:world_screen/command_line  ->  TemplateContainer
```

But the elements *inside* `CommandLine.uxml`(its `TextField`, `CompletionList`) keep `command_line` as their document id, not `world_screen`:

```
soulbound:command_line/text_field
soulbound:command_line/completion_list
```

In practice this means resolving a nested element is a two-step lookup: get the `TemplateContainer` from the parent root first, then query inside it using the *template's own* identifier, not a longer path under the parent's.

```csharp
VisualElement cmdLineRoot = root.Get<TemplateContainer>(COMMAND_LINE_ELEMENT); // soulbound:world_screen/command_line
TextField textField = cmdLineRoot.Get<TextField>(Identifier.Of("soulbound:command_line/text_field"));
```

### Modding (not yet supported)

The `namespace` segment of an `Identifier` exists specifically so that mod-provided UXML won't collide with base-game or other mods' identifiers. A modder's content will eventually be registered under their own namespace instead of `soulbound`. This isn't wired up yet; the generator currently only scans and registers content under the project's own UXML roots with the `soulbound` namespace hardcoded.