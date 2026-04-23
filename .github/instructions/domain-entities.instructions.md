---
applyTo: "src/Domain/**/*.cs"
---

# Domain Entity Conventions

- **Location**: Domain entities live under `src/Domain/DnDEntities/` or `src/Domain/Fight/`.
- **Interfaces**: Define an interface (e.g., `ICharacter`) with XML doc comments. Implementations use `<inheritdoc />`.
- **Default constructors**: Provide a parameterless constructor for deserialization, marked `[Obsolete]` if it shouldn't be called directly. Optionally provide a `bool withDefaults` constructor for creating entities with sensible defaults.
- **Marker interfaces**: `IHashable` (from `Memory.Hashes`) is used as a marker on domain types that participate in change-tracking.
- **Enums**:
  - Define in their own file (e.g., `AbilityEnum`, `DamageTypeEnum`).
  - Companion static class `{EnumName}Extensions` with a `public readonly static {EnumName}[] All = Enum.GetValues<{EnumName}>();` field.
  - Extension methods for display formatting (e.g., `ShortName()`, `ToReadableString()`).
- **Custom attributes**: Used on enum values for metadata (e.g., `AbilityAttribute`, `DamageFactorAttribute`). Retrieved via `EnumExtensions.GetAttribute<T>()` from the `Extensions` project.
- **Collections**: Domain collection types wrap `Dictionary<Guid, T>` or similar, with a `Values` accessor. Constructed with `bool withDefaults` for populating defaults.
- **No persistence assumptions**: Domain entities are persistence-agnostic. Repository interfaces (`ICharacterRepository`) live in the domain, implementations elsewhere.
- **Validation**: Validators are separate classes in a `Validation/` subfolder, not inline in the entity. See the validators instruction file.
- **Mapping**: Mapster `TypeAdapterConfig` registrations live in `Mapping/MappingConfigurations.cs` per feature area, registered via `IServiceCollection` extension methods.

## `record` vs `class`

- Use `record` for **immutable value types** — small, identity-less types whose equality is structural (e.g., `Wildcard`, `ScoreModifier`, `AppliedStatus`). These are never mutated after construction.
- Use `class` for **mutable entities** — types that carry identity (`Guid Id`), are mutated over time, or participate in change-tracking via `IHashable` (e.g., `Character`, `AbilityScore`, `HitPoints`).
