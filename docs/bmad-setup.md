# BMAD — Setup & Maintenance

How BMAD is installed in this repo, what belongs to you, and how to keep it current.
For day-to-day feature work see [bmad-feature-workflow.md](bmad-feature-workflow.md).

---

## What BMAD is

A library of ~90 *skills* — structured prompt workflows that guide an AI agent through
planning and implementation work. You invoke a skill by name in chat; the agent reads
its instructions and facilitates the process with you.

It is a **dependency**, not source code. Nearly everything it installs is regenerable.

## Prerequisites

| Tool | Why | Check |
|---|---|---|
| Node.js | runs the installer via `npx` | `node --version` |
| **uv** | **required** — skills execute through `uv run …/render_skill.py`. Without it, `bmad-build` halts on activation. | `uv --version` |

`uv` is not optional as of v6.11. If it's missing, ask an agent to "install and set up uv for me".

## Install / update

```powershell
npx bmad-method@latest install
```

Interactive. It detects the existing install and offers **Quick Update**, which
updates every module in place and keeps your answers.

Prompts you'll see, in order:

1. **Installation directory** — must be the repo root. Press Enter to accept the default;
   don't type anything, stray keystrokes get appended to the path.
2. **Install to this directory?** → Yes (it will report finding the existing `_bmad`)
3. **How would you like to proceed?** → Quick Update

Major-version bumps of external modules are held back deliberately. To accept one:

```powershell
npx bmad-method@latest install    # choose "Modify BMAD Installation"
# with: --pin bmb=v2.2.1
```

## Currently installed

| Module | Version | Notes |
|---|---|---|
| BMad Core | 6.11.0 | |
| BMad Method (bmm) | 6.11.0 | the main planning → ship pipeline |
| Test Architect (tea) | v1.23.1 | optional test-architecture workflows |
| CIS | v0.3.1 | creative / ideation workflows |
| BMad Builder (bmb) | v1.8.1 | held — v2.2.1 is a major release |
| WDS | v0.4.3 | **deprecated**, frozen; folding into bmm |

## Folder layout — what's yours

| Path | Files | Owner | Tracked? |
|---|---|---|---|
| `.agents/skills/` | ~1,720 | installer | regenerable |
| `_bmad/{core,bmm,tea,bmb,cis,wds}/` | ~70 | installer | regenerable |
| `_bmad/_config/manifest.yaml` | 1 | installer | **keep** — records exact versions + SHAs |
| `_bmad/config.toml` | 1 | installer | regenerated every install |
| **`_bmad/custom/`** | 4 | **you** | **keep** |
| **`_bmad-output/`** | — | **you** | **keep** — real content |

Only the bolded rows are authored. Everything else comes back from a reinstall.
`manifest.yaml` acts as the lockfile.

> If BMAD diffs are drowning your commits, gitignore `.agents/`, `.github/agents/`
> and `_bmad/*` while un-ignoring `_bmad/custom/` and `_bmad/_config/manifest.yaml`.
> Updates then show up as a one-line manifest change instead of ~640 paths.

## Customising a skill

Never edit files under `.agents/skills/` — they're overwritten on every update.
Overrides live in `_bmad/custom/` and are never touched by the installer:

| File | Scope | Committed? |
|---|---|---|
| `_bmad/custom/{skill-name}.toml` | team | yes |
| `_bmad/custom/{skill-name}.user.toml` | personal | no (gitignored) |

Merge rules: scalars override, arrays append, arrays-of-tables keyed by `code`/`id`
replace matches and append the rest.

Use the **`bmad-customize`** skill to write these for you rather than hand-authoring TOML.

### Override active in this repo

`_bmad/custom/bmad-spec.toml` widens `bmad-spec`'s `persistent_facts` to a glob.
The shipped default only looks at `{project-root}/project-context.md` (no glob), which
misses our file in `_bmad-output/`. Every other skill already globs `**/project-context.md`.

## One-time project setup

Run **`bmad-project-context`** (`PC`). It produces a small verified block inside a
root `AGENTS.md`: verified build/test commands, conventions that differ from defaults,
and known agent pitfalls.

It replaces the older `generate-project-context` and `document-project` skills, and will
offer to absorb an existing `_bmad-output/project-context.md` rather than leave it orphaned.

Re-run it later with intent `refresh` (re-verify), `record` (log a mistake an agent made),
or `audit` (prune stale rules).

## Deprecations to be aware of

| Old | Use instead |
|---|---|
| `bmad-generate-project-context` | `bmad-project-context` |
| `bmad-document-project` | `bmad-project-context` |
| `bmad-create-story`, `bmad-dev-story`, `bmad-quick-dev` | `bmad-build` |
| `bmad-create-prd`, `bmad-edit-prd`, `bmad-validate-prd` | `bmad-prd` |
| `bmad-create-architecture` | `bmad-architecture` |
| WDS agents (Freya, Mimir, Saga) | still work; no longer updated |

## Lost?

Invoke **`bmad-help`** (`BH`). It reads the current state and recommends the next skill.
