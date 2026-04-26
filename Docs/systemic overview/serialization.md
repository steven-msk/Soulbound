## Disclaimer
- this document only touches the architectural direction of serialization within the engine. **It is not a concrete implementation specification**
- alpha production will include a minimal, scoped implementation focused exclusively on settings serialization for simplicity reasons
- all world, entity, and gameplay serialization are out of scope for alpha and only described here as a concept and for future reference

## Top overview
- serialization is responsible for persisting and restoring engine state across sessions
- it works as a data transformation pipeline. It converts runtime objects into structured data and back
- the system should be designed to account domain separation, type-specific serialization policies and explicit versioning and compatibility control

## Design principles
- serialization is split into independent domains. For example the setting serialization is small-scale, full overwrite, and based on a key-value or a structured configuration, while the world serialization *(deferred to beta production)* is chunk-based and requires large-scale persistence.
- on top of all serialization policies there is meta serialization which mostly refers to global version tracking
- *each domain has its own pipeline and only shares common infrastructure such as versioning, codecs and format handling*
- **there is no single global serializer handling all types**. Serialization logic is split per system type with dedicated policies described as "codecs".
- each serializable type defines its own codec. This provides a bidirectional mapping between the serialized data and the target object. A codec is responsible for encoding runtime objects into structured data and decode them back.
- serialization does not directly target specific file formats (i.e. Json, binary etc). Instead, it uses an intermediate structure `SerializedData`. This is an abstraction which represents structured data like objects, primitives, arrays, and allows switching formatss without rewriting logic.
- **all serialized data must include a version identifier**. Versioning is made explicit and mandatory to every serializable object.
- *(planned)* migration pipeline will consist of a manager which handles serialized data migration. A migration is defined as an incremental transformation between versions.
    - note that older data formats are not deserialized directly. They are first transformed into the latest version and only then deserialized
- **all serialized references must use stable string identifiers. Numeric or positional identifiers are not allowed**.
- a central manager will coordinate codec access, version handling and read/write formatting. This manager does not contain any serialization logic or own domain-specific behavior

## Alpha production scope
- in alpha production there is only a one thing in the serialization system which should be included:
    - the settings serialization system, which will make use of JSON-based format, account for versioning, and have stable string-based keys.
    - implementation details: full overwrite on save, safe deserialization with fallback handling
- *world serialization, entity serialization, chunk-based persistence, cross-object references and the migration system are deffered to beta production*

## (Alpha) Settings serialization design
- this system will serialize all registered settings into a structured object, write to disk as JSON, completely overwriting the previous file
- at load, it will read the JSON file, validate the version field, and for each setting apply a deserialization rule:
    - if the value is present, it applies the value to the setting
    - if the value is missing, the setting value is set to default
    - if unknwon, its ignored
- a few constraints to keep in mind:
    - settings must define a unique string id (potentially an `Identifier`) and a default value
    - the system must tolerate missing fields and additional fields for forward compatibility

## Known issues
- schema evolution without migratios will break compatibility
- poor ID management will invalidate saved data
- tight coupling between runtime objects and serialized format will reduce flexibility