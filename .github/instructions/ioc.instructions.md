---
applyTo: "**/IoC/ServiceCollectionExtensions.cs,**/IoC/ServiceCollectionExtension.cs,**/MauiProgram.cs"
---

# Dependency Injection Conventions

- **Pattern**: Each project boundary exposes a `static class ServiceCollectionExtensions` in an `IoC/` folder.
- **Method naming**: `Register{ProjectOrFeature}Services(this IServiceCollection services)` or `Register{Feature}MappingConfigurations(this IServiceCollection services)`.
- **Return**: Always return `IServiceCollection` for chaining.
- **Lifetimes**:
  - `Singleton` for stateful services (`IFightContext`, `ICharacterRepository`, `IMapper`, `IJsonSerializer`, `IUserInteractionService`, `IDialogServiceProvider`).
  - `Scoped` for per-navigation/per-page services (`IStateFullNavigation`, edit contexts).
  - `Transient` for validators.
- **MauiProgram.cs** (composition root):
  - `AddMauiBlazorWebView()` + `AddMudServices()` for Blazor MAUI Hybrid.
  - `AddValidatorsFromAssemblyContaining<CharacterValidator>()` for FluentValidation auto-registration.
  - `ConfigureMediator(...)` for UndoableMediator with explicit assembly scanning (DnDActions + DnDQueryPrompter assemblies).
  - Chain all project `Register*` extension methods.
- **Mapping registrations**: Mapster `TypeAdapterConfig` setup lives in `Mapping/MappingConfigurations.cs` and is called via extension method chaining from the composition root.
