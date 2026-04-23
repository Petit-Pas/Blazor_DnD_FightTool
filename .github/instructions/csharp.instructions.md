---
applyTo: "**/*.cs"
---

# C# Conventions

- **Target framework**: .NET 10 (`net10.0`), SDK 10.0.100.
- **Language**: C# with `ImplicitUsings` and `Nullable` enabled in all projects.
- **Namespaces**: File-scoped namespaces everywhere (`namespace Foo;`).
- **XML docs**: Use `///` XML doc comments on public and internal types and members. Use `<inheritdoc />` when implementing an interface member documented in the interface or when overriding a virtual member.
- **Naming**: PascalCase for types, methods, properties. `_camelCase` for private fields. Interfaces use `I` prefix (e.g., `ICharacter`, `IMapper`, `IFightContext`).
- **Root namespaces**: All projects follow `DnDFightTool.{Layer}.{ProjectName}`.
  - Domain: `DnDFightTool.Domain.{ProjectName}` (e.g., `DnDFightTool.Domain.DnDEntities`, `DnDFightTool.Domain.Fight`)
  - Business: `DnDFightTool.Business.{ProjectName}` (e.g., `DnDFightTool.Business.DnDActions`, `DnDFightTool.Business.DnDQueries`)
  - Infrastructure: `DnDFightTool.Infrastructure.{ProjectName}` (e.g., `DnDFightTool.Infrastructure.Extensions`, `DnDFightTool.Infrastructure.Mapping`)
  - UI: `DnDFightTool.UI.{ProjectName}` (e.g., `DnDFightTool.UI.SharedComponents`, `DnDFightTool.UI.DnDEntitiesBlazorComponents`, `DnDFightTool.UI.DnDQueryPrompter`)
- **Constructors**: Never use primary constructors. Always use standard constructors.
- **Collections**: Use collection expressions (`[]`) where appropriate.
- **Properties**: Auto-properties with initializers (e.g., `public Guid Id { get; set; } = Guid.NewGuid();`).
- **Nullability**: Nullable reference types enabled. Use `?` for nullable, `!` for null-forgiving only when certain.
- **Exceptions**: Throw `ArgumentException` / `NullReferenceException` / `InvalidOperationException` with descriptive messages including the handler type name.
- **IoC pattern**: Each project boundary exposes a `ServiceCollectionExtensions` class in an `IoC` folder with extension methods on `IServiceCollection`.
- **Obsolete**: Mark deserialisation-only constructors with `[Obsolete("Should not be used, only for deserialization")]`.
