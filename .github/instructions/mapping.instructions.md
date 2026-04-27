---
applyTo: "**/Mapping/MappingConfigurations.cs"
---

# Mapping Configuration Conventions (Mapster)

- **Library**: Mapster via custom `IMapper` / `Mapper` wrapper in the `Mapping` infrastructure project.
- **Location**: Each feature area has a `Mapping/MappingConfigurations.cs` file next to the entities.
- **Visibility**: `internal static class MappingConfigurations` with `internal static IServiceCollection Register{Feature}MappingConfigurations(...)`.
- **Config pattern**: Use `TypeAdapterConfig<TSource, TTarget>.NewConfig()` for custom mapping rules.
- **Duplication**: Use `.IgnoreWhenDuplicating(x => x.Id)` to skip identity properties during `Clone()` (but not `Copy()`).
- **Dictionary-keyed-by-Id collections**: Any `class MyCollection : Dictionary<Guid, T>` where `T` uses `IgnoreWhenDuplicating(x => x.Id)` MUST also register a `TypeAdapterConfig<MyCollection, MyCollection>` mapping that uses `AfterMapping` to rebuild the dictionary from scratch — iterating `src.Values`, mapping each item individually via `Adapt<T>()`, then calling `dest.Add(mappedItem)`. This prevents key/value Id desync when the collection is cloned (Mapster copies dictionary keys verbatim but maps values with the new Id).
- **Clone vs Copy**:
  - `Clone<T>()` — creates a new instance with new Ids (for duplicating entities).
  - `Copy<T>()` — creates an exact copy including Ids (for undo snapshots).
- **Registration**: Called from a parent `RegisterDnDEntitiesMappingConfigurations()` extension method that chains all feature-specific registrations.
