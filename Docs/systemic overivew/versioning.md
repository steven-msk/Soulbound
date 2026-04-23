## Disclaimer
- this doc defines the versioning model and compatibility expectations of the game as a project
- it applies to both game release iterations and serialized data version tracking
- alpha prod will implement basic version tracking only, without full compatibility guarantees

## Top overview
- versioning provides a structured timeline of game evolution and a reference point for serialization compatibility
- the system is convention-driven, not enforced by strict rules or standards
- versions are used to identify builds, gate compatibility and drive migration logic

## Version structure
- **see [semantic versioning](https://semver.org) for a more structured doc**

## Compatibility model
- backward compatibility means new versions can read data from older versions
- forward compatibility means older versions can read data from newer versions
    - backward compatibility is supported through migrations and achieved through the versioned serialized data and the migration pipeline
    - forward compatibility is not guaranteed, and generally avoided due to unknown or inconsistent change and loss of meanings
- post-1.0, backward compatibility becomes a design goal, and the migration system should be required for all schema changes Forward compatibility can remain unsupported or limited

## Serialization versioning
- **the game's version is different from the serialized data version**
- serialization evolves at a different pace than game releases, and multiple game versions may share the same data scheme.
- a single release may introduce multiple schema changes during development
- you may optionally define a conceptual format for serialization version after the game version (e.g. 0.1.0 uses serialization version 2, 0.2.0 uses serialization version 3)
- this mapping is internal, and not really required for alpha

## Known issues
- versioning doesnt prevent incompatibility by itself
- missing migrations, so old data may become unusable
- forward compatibility is unreliable