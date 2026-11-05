---
name: sync-course-diagrams
description: >-
  Inspects recent git commits, branches, or code changes in the repository,
  identifies modified classes and architecture, updates the corresponding
  PlantUML (.puml) class and sequence diagrams, recompiles them to PNG,
  and syncs them into the course report .odt file. Use whenever the user asks to
  update diagrams in the course report from git commits, branches, or recent changes.
---

# Sync Course Diagrams from Git Commits

Use this workflow to automatically align the course report UML diagrams with the actual codebase and git history.

## Workflow Steps

### 1. Analyze Git Commits / Working Tree

1. Check current branch and git status:
   ```powershell
   git status
   ```
2. Inspect the specified commit(s) or branch diff:
   ```powershell
   git log -n <count> --stat
   # or
   git diff <target_commit>..HEAD --stat
   ```
3. For each modified file in `Assets/Scripts/`, consult [diagram_mapping.md](./references/diagram_mapping.md) to identify which `.puml` diagrams are affected.

### 2. Identify Architectural & Class Changes

Examine the diff (`git show <commit>` or `git diff`) for:
- **New or removed classes/interfaces** (e.g. deletion of `IInventoryItem`).
- **Modified constructors & dependencies** (e.g. `PauseManager` injected into FSM states).
- **Public API changes** (e.g. `Observe(StatType)` added for R3 reactive migration, removed `priority` from `StatModifier`/`ModifierData`).
- **Relocated scripts / namespaces** (e.g. scripts moved to feature folders `Combat/`, `Items/`, `Weapons/`).

### 3. Update the Affected `.puml` Files

1. Locate the diagram source files in:
   - `Diagrams/` (or `scratch/extracted/diagrams/puml/`)
   - `scratch/extracted/usecase_seq/puml/`
2. Apply surgical updates to the `.puml` files:
   - Add/remove member signatures.
   - Update dependency and association arrows (`-->`, `..>`, `<|--`, `<|..`).
   - Keep existing `skinparam` styling and color schemes intact.

### 4. Recompile Diagrams and Update the .ODT Report

Run the automation helper script:
```powershell
python .agents/skills/sync-course-diagrams/scripts/sync_diagrams.py
```

The script automatically:
1. Discovers `plantuml.jar` and renders all `.puml` files to high-DPI PNGs with `-charset UTF-8`.
2. Matches each diagram to its embedded image inside `Tsybrivskyi_course_updated.odt` (Рис. 1–26).
3. Repacks the `.odt` archive according to ODF standards (`mimetype` uncompressed).
4. Packages updated zip bundles (`diagrams_updated.zip` and `uml_usecase_and_sequence_updated.zip`).

### 5. Verify Deliverables

1. Verify that `Tsybrivskyi_course_updated.odt` opens without corruption.
2. Confirm that `diagrams_updated.zip` and `uml_usecase_and_sequence_updated.zip` contain the newest PNG and PUML files.
3. Report the summary of changed diagrams and classes to the user.
