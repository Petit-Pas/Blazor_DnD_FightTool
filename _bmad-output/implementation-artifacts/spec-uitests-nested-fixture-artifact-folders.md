---
title: 'Nested-fixture screenshot folders'
type: 'feature'
created: '2026-09-29'
status: 'done'
route: 'one-shot'
---

# Nested-fixture screenshot folders

## Intent

**Problem:** UI test files now hold an empty static container class with nested `SequentialScenarioFixture`/`IsolatedScenarioFixture` classes per sub-case. NUnit reports a nested class as `Outer+Inner`, so screenshots would land in one flat `Outer+Inner` folder instead of mirroring the test tree.

**Approach:** `ApplicationFixture.GetScenarioFolder()` splits the relative class name on `+` as well as `.`, so each nested fixture gets its own folder below its container class.

## Suggested Review Order

**Artifact folder resolution**

- Entry point: `+` joins `.` as a segment separator, nesting inner-fixture folders.
  [`ApplicationFixture.cs:189`](../../uitests/Framework/ApplicationFixture.cs#L189)

- Doc states the nested-folder behaviour.
  [`ApplicationFixture.cs:171`](../../uitests/Framework/ApplicationFixture.cs#L171)

**Peripherals**

- Stale doc fixed: the fixture is linked into every UI test project, not only Scratch.
  [`AssemblyFixture.cs:8`](../../uitests/Framework/AssemblyFixture.cs#L8)
