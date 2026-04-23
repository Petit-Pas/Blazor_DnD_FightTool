---
applyTo: "src/Domain/Fight/**/*.cs"
---

# Fight Domain Conventions

- **FightContext**: Singleton `IFightContext` holds the active fight state. Provides indexer `this[Guid id]` to access `FightingCharacter` by Id (returns `null` if not found).
- **FightingCharacter**: Wraps a `Character` via composition (not inheritance). Implements `ICharacter` by delegating to the inner `_character`. Adds fight-specific state (`InitiativeRoll`).
- **Character lifecycle**: Players are added by reference. Monsters are cloned (`_mapper.Clone(character)`) so the original template is not mutated.
- **Events**: Use standard `EventHandler<T>` pattern (e.g., `OnActiveFighterChanged`, `OnFighterRemoved`, `OnFighterUpdated`). `OnFighterUpdated` is fired via `NotifyFighterUpdated(Guid)` when a fighter's state is mutated in-place (same object reference) — this is needed because Blazor won't detect changes on the same reference.
- **Domain extensions**: Fight-specific extension methods on domain types live in `DomainExtensions/{Feature}/` (e.g., `DamageRollTemplateExtensions`, `HitPointsExtensions`, `MartialAttackTemplateExtensions`).
- **Sorting**: Static `Func<FightingCharacter, (int, int)>` sort keys (e.g., `InitiativeSortKey`) for ordering fighters.
- **Copy**: `FightingCharacter.Copy(IMapper mapper)` creates a deep copy including the underlying character.
