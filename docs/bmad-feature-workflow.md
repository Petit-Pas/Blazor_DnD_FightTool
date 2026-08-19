# BMAD — Building a Feature

Which skills to run, in what order, to get from an idea to shipped code.
For install and maintenance see [bmad-setup.md](bmad-setup.md).

---

## Pick a lane first

Not every change deserves the full pipeline. Match the lane to the size of the work.

| Lane | When | Skills |
|---|---|---|
| **Direct** | bug fix, tweak, refactor, small addition | `bmad-build` |
| **Spec-first** | one clear feature; you know what you want | `bmad-spec` → `bmad-build` |
| **Full pipeline** | new product area, multiple epics, real unknowns | everything below |

Over-planning a two-file change wastes more time than it saves. Under-planning a
multi-epic feature costs more. When unsure, ask **`bmad-help`** (`BH`).

---

## Lane 1 — Direct

> Invoke `bmad-build`

One loop: clarify intent → plan → implement → review → present. It reads the repo's
existing architecture, patterns and conventions and follows them.

This is the official implementation method — it replaced `create-story`, `dev-story`
and `quick-dev`.

Optionally follow with **`bmad-code-review`** (`CR`) for an extra adversarial pass.

---

## Lane 2 — Spec-first

> `bmad-spec` (`SPC`) → `bmad-build` (`BD`)

**`bmad-spec`** distils any input — a brain dump, a transcript, a doc, a mixed pile of
sources — into a `SPEC.md` contract plus companions. It locks the **what** before the
**how**, which is where most feature work goes wrong.

Output: `_bmad-output/specs/spec-{slug}/`

It can also break the spec into stories, and has a validation mode for checking an
existing spec.

---

## Lane 3 — Full pipeline

### Phase 1 — Shape the idea (optional)

| Skill | Code | Purpose |
|---|---|---|
| `bmad-brainstorming` | `BP` | facilitated ideation when you're stuck |
| `bmad-forge-idea` | `FI` | persona-driven interrogation until the idea hardens or dies cheaply |
| `bmad-deep-recon` | `RS` | decision-grade research (market, technical, competitive, …) |

### Phase 2 — Plan

| Step | Skill | Code | Required | Output |
|---|---|---|---|---|
| 1 | `bmad-product-brief` *or* `bmad-prfaq` | `CB` / `WB` | no | product brief |
| 2 | `bmad-prd` | `PRD` | **yes** | PRD |
| 3 | `bmad-ux` | `CU` | no — but do it if UI is central | UX design |
| 4 | `bmad-architecture` | `CA` | **yes** | architecture spine |
| 5 | `bmad-create-epics-and-stories` | `CE` | **yes** | epics + stories |
| 6 | `bmad-sprint-planning` | `SP` | **yes** | sprint status file |

Artifacts land in `_bmad-output/planning-artifacts/` (sprint status goes to
`_bmad-output/implementation-artifacts/`).

**`bmad-product-brief` vs `bmad-prfaq`** — the brief is the gentler path when you're
already committed to the concept. PRFAQ works backwards from a press release and will
try to talk you out of it; use it when the concept still needs proving.

**`bmad-architecture` on a brownfield repo** like this one *ratifies the existing
codebase* rather than inventing a new design. It produces the invariants that keep
features, epics and stories from diverging.

**`bmad-sprint-planning`** is a readiness gate — it returns PASS / CONCERNS / FAIL on
whether the planning is actually implementable, then emits the tracking file the build
loop follows.

### Phase 3 — Ship

| Step | Skill | Code | Required |
|---|---|---|---|
| 1 | `bmad-build` | `BD` | **yes** |
| 2 | `bmad-code-review` | `CR` | no |
| 3 | `bmad-qa-generate-e2e-tests` | `QA` | no |
| 4 | `bmad-retrospective` | `ER` | no — at epic end |

`bmad-build` runs once per story, driven by the sprint status file. It has a built-in
review; `CR` is an optional extra layer on top.

---

## Useful at any point

| Skill | Code | Use when |
|---|---|---|
| `bmad-help` | `BH` | you don't know what's next |
| `bmad-sprint-planning` (status) | `SS` | "where are we?" — risks, open items, next action |
| `bmad-review` | `RV` | review anything before it ships — diff, branch, PRD, spec, prose |
| `bmad-advanced-elicitation` | `AE` | push a just-produced draft past its first version |
| `bmad-correct-course` | `CC` | something significant changed mid-sprint |
| `bmad-checkpoint-preview` | `CK` | guided human walkthrough of a commit / branch / PR |
| `bmad-party-mode` | `PM` | you want multiple agent perspectives at once |

**`bmad-review`** is the one to reach for on a bloated or LLM-slop draft. It runs
whichever lenses fit the content — adversarial, edge-case, verification-gap, structure,
prose — and reports findings in one shape.

**`bmad-correct-course`** is the pressure valve. It may recommend redoing the PRD,
revisiting architecture, or re-planning the sprint. Better than quietly building the
wrong thing.

---

## Testing track (optional, TEA module)

Runs alongside the main pipeline rather than after it.

**During planning:** `bmad-testarch-test-design` (`TD`) → `bmad-testarch-framework` (`TF`) → `bmad-testarch-ci` (`CI`)

**During implementation:** `bmad-testarch-atdd` (`AT`) → `bmad-testarch-automate` (`TA`) → `bmad-testarch-test-review` / `bmad-testarch-nfr` → `bmad-testarch-trace` (`TR`)

`AT` generates red-phase acceptance tests *before* implementation — the TDD entry point.

Artifacts land in `_bmad-output/test-artifacts/`.

---

## Where everything lands

```
_bmad-output/
├── planning-artifacts/        brief, PRD, UX, architecture, epics & stories
│   └── research/              deep-recon reports
├── implementation-artifacts/  sprint status, build output, retrospectives
├── test-artifacts/            TEA module output
├── specs/spec-{slug}/         bmad-spec kernels
├── brainstorming/
└── forge/
```

---

## This repo's conventions

### Two planning systems coexist

Features may be defined by **either** system — both are valid:

| System | Location |
|---|---|
| Spec Kit | `specs/NNN-kebab-name/` |
| BMAD | `_bmad-output/specs/spec-NNN-kebab-name/` |

`NNN` is **one shared sequence across both**. Before creating a feature in either system,
check both folders for the highest number and take the next one. Never reuse a number,
even one the other system owns.

Highest in use: **004** → next is **005**, whoever creates it.

Don't migrate or mirror features between systems, and don't assume the system you're
working in owns the active feature — check both.

> `specs/` at root is Spec Kit; `_bmad-output/specs/` is BMAD. Same word, different trees.

### Rules agents must follow

Agents working here must also respect:

- `AGENTS.md` (root) — the verified context block, once `bmad-project-context` has run
- `.github/instructions/*.md` — 15 path-scoped rule files (`applyTo` globs), covering
  C#, Blazor components/code-behind/CSS, commands, queries, domain entities, validators,
  mapping, IoC, and tests
- `.github/skills/` — project-specific skills for `undoable-mediator` and `dnd-logging`

The instruction files are path-scoped, so an agent only sees them when touching a matching
file. Cross-cutting rules belong in `AGENTS.md`.
