# AGENTS.md

This file is the first thing Codex should read when working in this repository.

## Always Read First

Before changing code, scenes, prefabs, ScriptableObjects, or project settings:

1. Read `CODEX.md`.
2. Read `code-style.md`.
3. Check the current git status and do not overwrite user changes.

For planning-heavy requests, architecture decisions, or when the user says "grill me", also read `GRILL ME.md` before proposing a plan.

## Project-Specific Defaults

- This is a Unity game project, not a web/backend project.
- Main custom runtime code lives mostly in `Assets/Scripts/`.
- ScriptableObject definitions live in `Assets/Scriptable Objects/Scripts/`; authored data assets live under `Assets/Scriptable Objects/`.
- Current architecture is a work in progress: MonoBehaviours, ScriptableObjects, VContainer, singleton-style global services, coroutines, events, NightPool, DOTween, UGUI, and TextMeshPro all coexist.
- Prefer small changes that fit the existing code over broad architecture rewrites.
- Do not edit imported/vendor code unless the user explicitly asks. This includes `Assets/Plugins/`, `Assets/JMO Assets/`, `Assets/TextMesh Pro/Examples & Extras/`, and other third-party asset folders.
- Do not manually edit generated Unity files such as `Assets/Scripts/Player/PlayerInputActions.cs`; change `Assets/PlayerInputActions.inputactions` in Unity instead.
- Keep Unity `.meta` files with assets when creating/moving Unity assets. For pure root markdown docs, no `.meta` file is needed.
