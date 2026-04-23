# Constitution Update Checklist

Run through this checklist whenever you consider modifying `constitution.md`.

## When to update the constitution

Update the constitution when a decision affects the **entire project** going forward, not just one feature. Examples:
- Upgrading the runtime or a core library (e.g., .NET, MudBlazor, UndoableMediator)
- Introducing a new cross-cutting infrastructure pattern
- Renaming or restructuring a layer or project
- Changing a mandatory lifetime for a service category
- Adopting a new testing library or dropping one

Do **not** update the constitution for feature-specific decisions — those belong in the spec or plan for that feature.

---

## Checklist before editing

- [ ] Is this change truly project-wide, or only relevant to one feature/layer?
- [ ] Have you confirmed the change does not contradict an existing rule?
- [ ] If a library version is changing, is the new version already in `global.json` or a `.csproj`?
- [ ] Has the impacted `.github/instructions/*.instructions.md` file also been updated to match?
- [ ] Will existing commands/queries/entities need to be migrated? (Document the migration path in the relevant spec.)
- [ ] Have you updated the "Technology Stack" table (Section 1) if a package version changed?
- [ ] Have you updated the "Where to Create New Files" table (Section 3) if a folder was added, removed, or renamed?
- [ ] Have you updated the layer diagram (Section 2) if a new project was added?

---

## After editing

- [ ] Review all open specs and plans — do any contradict the updated rule?
- [ ] Notify the agent to re-read the constitution before the next `/plan` or `/tasks` run.
