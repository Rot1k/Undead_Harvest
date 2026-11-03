# CODEX.md

This document describes the current state of the Undead Harvest Unity project for Codex.

## Project Context

Undead Harvest is a 2D roguelike/survival game inspired by Brotato. The player is a farmer fighting waves of undead with firearms, gardening tools, passive items, status effects, and between-wave shop upgrades.

The project is actively in development. Some systems are incomplete, partly migrated, or named inconsistently. Treat the current code as the source of truth, and avoid sweeping cleanup unless the user asks for it.

## Technology Stack

- Unity `6000.5.1f1`.
- Universal Render Pipeline `17.3.0`, 2D-focused project setup.
- C# with MonoBehaviours, plain C# runtime classes, ScriptableObjects, events, and coroutines.
- VContainer `1.17.0` for scene dependency injection.
- NightPool (`com.nighttraincode.nightpool`) for pooled runtime objects.
- Unity Input System `1.18.0`; generated wrapper is `Assets/Scripts/Player/PlayerInputActions.cs`.
- UGUI and TextMeshPro for UI.
- DOTween is imported under `Assets/Plugins/Demigiant/DOTween` and is used in UI animation, especially `WaveEndWindow`.
- Cinemachine, Unity Visual Scripting, Timeline, and standard Unity modules are installed, but not all are core gameplay dependencies.

## Important Folders

`Assets/Scripts/` is the main custom runtime code folder:

```text
Assets/Scripts/
+-- Collectible Items/          # Pickups such as ExpCoin
+-- Enemies/                    # Enemy base class, variants, visuals, status effect manager
+-- Interfaces/                 # Small gameplay interfaces and Rarity enum
+-- Player/                     # Movement, health, level, input wrapper output, stat system
|   +-- StatSystem/             # Stat, StatModifier, StatType, PlayerStats
+-- Systems/                    # Waves, equipment, combat, loading, pause, wallet, audio
|   +-- Audio/
|   +-- DI Containers/          # SceneLifetimeScope
+-- UI/                         # HUD, menu, shop, stats, wave UI
|   +-- LevelUpWindowUI/
|   +-- ShopUI/
+-- Weapons/                    # Weapon runtime, holder, visuals, shot behaviors
    +-- WeaponBehaviors/
```

ScriptableObject code and data are separate:

```text
Assets/Scriptable Objects/
+-- Scripts/                    # SO class definitions
+-- Enemies/                    # Enemy stats assets
+-- PassiveItems/               # Passive item assets
+-- PlayerStats/                # Player base stat config
+-- StatusEffectModifiers/      # Effect modifier assets
+-- StatusEffects/              # Burn, freeze, poison, bleed, etc.
+-- Waves/                      # WaveConfigSO assets
+-- Weapons/                    # Weapon assets by weapon family and rarity
```

Scenes in build order:

1. `Assets/Scenes/MainMenuScene.unity`
2. `Assets/Scenes/LoadingScene.unity`
3. `Assets/Scenes/GameScene.unity`

Core prefabs live under `Assets/Prefabs/`: `Player.prefab`, enemies, projectiles, UI slots/windows, and weapon prefabs.

## Runtime Pipeline

`GlobalManagersBootstrap` uses `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]` to create `GameInput` before scene load if it does not already exist. Audio manager bootstrapping exists as commented-out code.

`GameInput` is a `DontDestroyOnLoad` singleton-style MonoBehaviour. It owns `PlayerInputActions`, exposes `MovementVector`, and raises `OnPause` / `OnCancel`.

Scene transitions go through `Loader`:

- `Loader.Load(targetScene)` stores the target and loads `LoadingScene`.
- `LoaderCallback.LoaderCallback()` loads the stored target scene and invokes `OnAfterSceneLoad`.

`GameScene` should contain a `SceneLifetimeScope`. Its current job is to register scene components in hierarchy:

- `WavesManager`
- `WaveEndWindow`
- `PauseManager`
- `EquipmentManager`
- `WalletManager`

After the VContainer container is built, `SceneLifetimeScope` injects all root GameObjects in the scene.

`Bootstrap.Start()` initializes the scene systems, registers the game flow states, then enters `StartingState`. `StartingState` calls `WavesManager.StartGame()`, which waits one frame before starting the first wave so listeners can subscribe. `WavesManager` delegates enemy creation to `EnemySpawner`; pooled enemies are injected manually through `IObjectResolver.InjectGameObject`.

`WeaponsHolder.Start()` subscribes to `EquipmentManager` events, calls `_equipmentManager.OnInit()`, spawns starting weapons, arranges weapons around the player, and resets weapon state on wave completion.

Wave flow:

1. `WavesManager.OnWaveStarted`
2. Enemy spawn routines run for each `WaveEntry`.
3. Time expires or boss dies.
4. `WavesManager.EndCurrentWave()`
5. `OnWaveCompleted` or `OnAllWavesCompleted`
6. `PauseManager` pauses when the wave ends.
7. `WaveEndWindow`, level-up UI, and shop UI decide what to show.
8. Closing the shop calls `WavesManager.StartNextWave()`.

## Architecture Notes

- Scene services are registered with VContainer. `EnemyRegistry` is a scene singleton service injected into enemies, the enemy spawner, and weapons; it tracks active enemies for per-type caps and target search.
- `CombatSystem` remains a static, stateless damage calculation helper. It has no owned runtime state to manage.
- Use `[Inject] public void Construct(...)` for MonoBehaviours that need scene services.
- Do not use injected dependencies in `Awake`; cache local components there. Use injected services in `Start`, in the inject method, or later lifecycle methods.
- If a NightPool-spawned object needs VContainer dependencies, the spawner must inject it manually using `IObjectResolver.InjectGameObject`.
- Plain runtime state classes such as `HealthSystem`, `Stat`, `StatModifier`, `PassiveItemInstance`, and `StatusEffectInstance` are not MonoBehaviours.
- ScriptableObjects are configuration/data. Keep gameplay service logic in MonoBehaviours or plain C# services unless there is a strong reason.
- Many gameplay interactions are event-driven. Subscribe in `Start`/`OnEnable` only when dependencies are ready, and unsubscribe in `OnDestroy`/`OnDisable`.

## Gameplay Systems Map

- Player stats: `PlayerStats` builds runtime `Stat` objects from `PlayerStatsSO`, applies/removes `StatModifier`s, and broadcasts `OnStatChanged`.
- Health: `HealthSystem` is a plain C# class with `OnHealthChanged`, `OnDamaged`, `OnHealed`, and `OnDead`.
- Enemies: `Enemy` owns runtime stats, health, movement toward the player, death handling, exp coin spawn, and status effect application. Variants include `RangerEnemy`, `DashEnemy`, and `BossEnemy`. `EnemyRegistry` tracks live enemies and counts them by `EnemyStatsSO`; wave caps apply per enemy type.
- Waves: `WaveConfigSO` contains wave entries, spawn intervals, max enemies, duration, and boss-wave flag.
- Weapons: `WeaponSO` configures weapon stats, prefab, rarity, scaling, union result, and on-hit effects. Runtime weapons inherit from `Weapon`.
- Weapon spawning: `EquipmentManager` owns equipped weapon/item data; `WeaponsHolder` turns weapon slots into pooled weapon GameObjects.
- Items: `PassiveItemSO` defines stat modifiers; `PassiveItemInstance` gives each equipped item a runtime `Guid` source id so modifiers can be removed by source.
- Shop/UI: shop windows roll items from `InventoryItemsPoolSO`, use `RarityConfigSO`, spend via `WalletManager`, and update equipment/inventory UI.
- Descriptions: `WeaponSO.Description.cs`, `PassiveItemSO.Description.cs`, and `ItemDescriptionBuilder` form templated TextMeshPro-rich descriptions.
- Targeting: weapons search the full active enemy registry for the nearest target in attack range. They keep the current target while a different candidate is observed for the configured switch delay (`Weapon._targetSwitchDelay`); `_targetGraceTime` delays clearing a target when no candidate is found.
- Pooled enemy registration happens both in `Enemy.OnSpawn()` and after DI in `Enemy.Construct()`, because NightPool may activate an enemy before the spawner injects its dependencies.
- Shop rolls choose a weapon/passive category, roll a rarity from `RarityConfigSO`, and prefer matching items. If that category contains no item of the rolled rarity, the shop falls back to any item in that category. Rarity `StatMultiplier` is used by level-up stat offers, not shop pricing.
- `StatModifier` has no priority ordering. Flat, additive-percent, and multiplier modifiers are aggregated by type in `Stat.Recalculate()`.

## Current Behavior Notes

- Enemy spawn positions are random within the configured map rectangle and must be at least the configured minimum distance from the player. They are not guaranteed to be outside the camera view.
- `WaveEntry.MaxEnemiesInScene` is a per-enemy-type cap, not a total scene enemy cap. Reaching the cap skips that spawn attempt; the spawn routine continues on its normal interval.
- `WavesManager.DespawnAllEntities()` despawns pooled entities at wave end, including enemy and projectile pools, while intentionally skipping weapon and inventory-slot pools.
- Status effect ticking performs at most one tick per `Update()` call, then resets the tick timer. Long frames do not catch up missed ticks.
- `WaveConfigSO.WaveDuration` documents `-1` as infinite, but the current `WavesManager.RunWave()` ends a wave for any duration `<= 0`. Treat this as a known implementation/documentation mismatch if configuring infinite waves.
- `State.Update()` exists but the state machine does not call it. Current game-flow states transition through `Enter()` and events; add the update dispatch only when a state needs frame-based behavior.

## Editing Rules

- Prefer editing project code under `Assets/Scripts/` and SO code under `Assets/Scriptable Objects/Scripts/`.
- Avoid editing scenes, prefabs, and `.asset` files by hand unless the task specifically requires YAML-level changes and you understand the serialized references.
- Be careful with paths containing spaces: `Assets/Scriptable Objects`, `Assets/Scripts/Collectible Items`, `Assets/Scripts/Systems/DI Containers`, and `Assets/Violet Theme Ui`.
- Do not rename public serialized fields casually; Unity serialized references depend on field names.
- When moving or renaming Unity assets, preserve `.meta` files.
- Existing typos such as `Meele`, `GlobalManagaresBootstrap`, or `Constuct` may be part of serialized/component references. Fix only with a deliberate migration.
- Imported examples and third-party packages should be treated as vendor code.

## Verification

There are no obvious project-specific CLI tests configured. Prefer these checks when possible:

- Inspect C# compile errors in Unity/Rider if available.
- Use `dotnet`/IDE build only if the generated solution is reliable for the current Unity state.
- For logic-only edits, reason through event lifecycle and nullability carefully.
- For scene/prefab-dependent changes, tell the user when Unity Editor validation is still needed.

## Git / Generated Files

- Unity-generated `*.csproj` and `*.sln` are ignored; `.slnx` files exist in this repo.
- Do not commit or rely on `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `obj/`, `.vs/`, or build outputs.
- `Assets/Scripts/Player/PlayerInputActions.cs` is generated from `Assets/PlayerInputActions.inputactions`; do not manually edit the generated file.
