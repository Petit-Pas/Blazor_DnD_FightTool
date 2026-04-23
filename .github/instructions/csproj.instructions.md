---
applyTo: "**/*.csproj"
---

# Project File Conventions (.csproj)

- **Target framework**: `net10.0`.
- **SDK**: `Microsoft.NET.Sdk` for class libraries, `Microsoft.NET.Sdk.Razor` for Razor class libraries.
- **Standard properties**:
  ```xml
  <ImplicitUsings>enable</ImplicitUsings>
  <Nullable>enable</Nullable>
  ```
- **Root namespace**: Set `<RootNamespace>` explicitly when it differs from the project/folder name. All projects use `DnDFightTool.{Layer}.{ProjectName}`.
- **Test projects**: Include `<IsPackable>false</IsPackable>` and `<IsTestProject>true</IsTestProject>`.
- **Test packages**: NUnit, NUnit3TestAdapter, NUnit.Analyzers, Microsoft.NET.Test.Sdk, FluentAssertions, FakeItEasy, coverlet.collector.
- **InternalsVisibleTo**: Use `<InternalsVisibleTo Include="{TestProjectName}" />` for testing internal members.
- **Razor class libraries**: Use `<AddRazorSupportForMvc>true</AddRazorSupportForMvc>` with `<FrameworkReference Include="Microsoft.AspNetCore.App" />`.
- **No wildcard versions**: Pin package versions explicitly.
