# GRILL ME.md

Use this when the user wants to stress-test a plan/design, asks for planning before code, or says "grill me".

The goal is to expose hidden assumptions before implementation. Be direct, concrete, and grounded in the current Undead Harvest codebase.

## Pre-Flight

Before the first decision question, collect or infer baseline context:

- Target platform: desktop, mobile, web, console, or unknown.
- Runtime scale: expected enemy count, projectile count, pooled object count, UI update frequency, wave length, save/load scope.
- Existing systems involved: VContainer, NightPool, ScriptableObjects, Input System, DOTween, UGUI/TextMeshPro, coroutines, scene loader, audio managers.
- Previous attempts: what was already tried and why it failed.
- Hard constraints: do not break serialized scene/prefab references, public data asset format, current wave/shop flow, current controls, or user deadlines.

If the answer is visible in code, inspect code instead of asking. Then state the fact briefly and continue.

## Question Rules

- Ask one decision question at a time.
- Offer 2-4 concrete options when the UI/tooling supports it.
- Put the recommended option first and mark it as recommended.
- Each option needs a short trade-off.
- If the user answers with a custom direction, update the mental model and do not re-offer the same rejected recommendation.
- Do not ask questions that no longer matter because of an earlier answer.

If the special multiple-choice question tool is unavailable, ask a concise plain-text question with the same structure.

## What To Cover

Keep asking until these areas are covered for the requested change:

- Scope: what is included and what is explicitly out.
- API/data shape: class names, fields, ScriptableObject fields, serialized references, event names, enum values.
- Lifecycle: when objects are created, injected, enabled, pooled, reset, disabled, destroyed.
- Scene flow: which scene owns the object and how it survives or resets across `MainMenuScene`, `LoadingScene`, and `GameScene`.
- Pooling: whether objects are spawned through NightPool and whether they need manual VContainer injection.
- Stats/effects: how `PlayerStats`, `Stat`, modifiers, status effects, weapon scaling, and item descriptions interact.
- UI flow: HUD, wave end, level-up, shop, inventory, and win/lose windows.
- Edge cases: null configs, empty pools, zero/negative values, repeated subscriptions, pause/timeScale, all waves completed, boss wave, player death.
- Risks: what can break in existing scenes, prefabs, data assets, or event ordering.
- Verification: what can be checked from code and what needs Unity Editor playtesting.

## How To Push Back

Do not agree just to be agreeable. Push back only when there is a concrete contradiction with:

- Existing code.
- These docs.
- Unity/VContainer/NightPool lifecycle.
- Serialized Unity data safety.
- The user's own stated constraints.

When pushing back, cite the specific file or system. Keep it calm and practical.

## Stop Conditions

Stop grilling only when:

- The user says to stop and make the plan.
- The main decision tree is covered and no important branch remains open.
- The next step is clearly implementation and further questions would only restate settled choices.

After stopping, summarize the plan in implementation order, list risks, and name the files/systems likely to change.

