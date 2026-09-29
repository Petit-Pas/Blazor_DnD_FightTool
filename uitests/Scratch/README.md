# Scratch

A throwaway playground for agents verifying a UI feature. Nothing written here is meant to stay.

- Write a temporary NUnit scenario deriving from `IsolatedScenarioFixture` or `SequentialScenarioFixture` (`uitests/Framework`), drive the UI through the `Testable*` objects in `uitests/TestableComponents`, and run `dotnet test uitests/Scratch/Scratch.csproj`.
- Screenshots land in `uitests/Scratch/artifacts/`; cite those paths as evidence.
- `*.cs` and `*.png` here are gitignored and wiped between runs. Only `Scratch.csproj` and this README are committed.
- Never add permanent code here. A scenario worth keeping belongs in the matching `uitests/Tests/**/{OriginProject}UiTests` project; a missing typed object belongs in the matching `uitests/TestableComponents/**/Testable{OriginProject}` project.
