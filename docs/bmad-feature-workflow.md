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

## The personas

BMAD ships five named personas. They are **skills, not VS Code agents** — no separate
context window, no restricted toolset. Activating one layers an identity, a value
system and a menu on top of the current conversation.

| Persona | Skill | Menu | Reach for when |
|---|---|---|---|
| 📊 **Mary** — Business Analyst | `bmad-agent-analyst` | `BP` `MR` `DR` `TR` `TS` `CR` `UV` `CB` `WB` | you're still deciding *whether* to build it — research, competitive teardown, briefs |
| 📋 **John** — Product Manager | `bmad-agent-pm` | `PRD` `CE` `IR` `CC` | you know it's worth building and need requirements that survive contact with code |
| 🎨 **Sally** — UX Designer | `bmad-agent-ux-designer` | `CU` | UI is the substance of the feature, not a wrapper around it |
| 🏗️ **Winston** — System Architect | `bmad-agent-architect` | `CA` `IR` | independently-built parts risk diverging, or you want trade-offs instead of a verdict |
| 💻 **Amelia** — Senior Engineer | `bmad-agent-dev` | `BD` `QA` `CR` `SP` `ER` | you're implementing and want TDD discipline enforced rather than suggested |

### They are never automatic

No workflow skill activates a persona. Running `bmad-spec` → `bmad-build` →
`bmad-code-review` gives you zero persona switching — those skills are self-contained.

The dispatch only runs **persona → skill**, never the reverse. Each persona's menu
invokes workflow skills; no workflow skill reaches back for a persona.

### Activating one

Say the name, or ask for the role:

```
talk to Winston
I need the architect
hey Amelia, let's implement the next story
```

Activation resolves `customize.toml`, loads persistent facts, greets you, then either
**dispatches directly** (if your message already named an intent) or **presents the
numbered menu** and waits.

Once active the persona **carries through every subsequent skill call** until you
dismiss it. Winston stays Winston while `bmad-architecture` runs. Messages stay
prefixed with the icon so the active persona is visible at a glance.

To drop it: *"dismiss the persona"* / *"drop the persona"*.

### Persona vs. calling the skill directly

Both reach the same workflow. The difference is framing.

| | Persona route | Direct route |
|---|---|---|
| Entry | *"talk to Winston"* → menu → `CA` | *"create the architecture"* |
| Extra baggage | greeting, menu, in-character prose | none |
| Value system | principles enforced across the whole session | per-skill defaults |
| Best for | open-ended thinking, several related steps | one known deliverable |

**Use a persona** when you want a lens held consistently over multiple turns — pressure-
testing a design, or working through a planning phase where you'll invoke three or four
skills back to back.

**Skip it** when you know exactly which artifact you want. `bmad-build` directly is
fewer turns than Amelia → menu → `BD`.

### What each persona already knows here

All five load this as a persistent fact on activation:

```toml
persistent_facts = ["file:{project-root}/**/project-context.md"]
```

That glob picks up [`_bmad-output/project-context.md`](../_bmad-output/project-context.md)
— the 42 rules covering MudBlazor 9 breaking changes, UndoableMediator (never MediatR),
`IMapper.Clone` vs `.Copy` semantics, `PropertyTargetedValidator`, strict FakeItEasy
fakes. So a persona starts already knowing this repo's non-obvious constraints; a bare
skill invocation relies on the path-scoped `.github/instructions/*.md` files instead.

### Customising a persona

`customize.toml` in each skill folder is **overwritten on every BMAD update** — don't
edit it. Layer overrides instead:

```
{skill-root}/customize.toml                    defaults  (do not edit)
_bmad/custom/bmad-agent-dev.toml               team
_bmad/custom/bmad-agent-dev.user.toml          personal
```

Merge rules: scalars override, `persistent_facts` / `principles` / `activation_steps_*`
append, menu items merge by `code`. Name and title are fixed — build a custom agent if
you need a different identity.

Run **`bmad-customize`** (`BC`) rather than hand-writing the TOML.

### Multiple perspectives at once

**`bmad-party-mode`** (`PM`) is the exception to one-persona-at-a-time — it orchestrates
a roundtable across several personas. Useful for a decision where you actively want the
architect and the PM to disagree in front of you.

> The WDS module ships its own separate cast (Saga, Freya, Mimir, Idun) as real
> `.github/agents/*.agent.md` files — different mechanism, different tree. Don't confuse
> them with the five above.

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
