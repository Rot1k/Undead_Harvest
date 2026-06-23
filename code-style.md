# code-style.md

Style guide for C# code in Undead Harvest. This is based on the current project code plus `.editorconfig`.

## Scope

Applies to custom project code:

- `Assets/Scripts/**/*.cs`
- `Assets/Scriptable Objects/Scripts/**/*.cs`

Does not apply to imported packages, generated files, or sample code:

- `Assets/Scripts/Player/PlayerInputActions.cs`
- `Assets/Plugins/**`
- `Assets/JMO Assets/**`
- `Assets/TextMesh Pro/Examples & Extras/**`

## File Structure

- Current project classes mostly have no namespace. Do not introduce namespaces in one-off edits unless the user asks for a larger organization pass.
- Keep one primary type per file when practical.
- `using` directives stay outside namespaces.
- Braces use Allman style.
- Indentation is 4 spaces.
- Prefer explicit accessibility modifiers for non-interface members.
- Prefer explicit types over `var`, matching `.editorconfig`.
- Keep serialized Unity fields stable; renaming a `[SerializeField]` field can break inspector data.

## Naming

- Classes, structs, enums, methods, properties, and events use `PascalCase`.
- Interfaces use `I` prefix: `IDamageable`, `ICollectible`, `IWeaponBehavior`.
- Private fields use `_camelCase`.
- Serialized private fields use `[SerializeField] private Type _fieldName;`.
- Constants currently use `UPPER_SNAKE_CASE` in several files, especially animator and PlayerPrefs keys. Continue that style for new constants.
- Local variables and parameters use `camelCase`.
- Boolean members should read naturally: `IsPaused`, `IsDead`, `CanUnion`, `HasTarget`, `TrySpendMoney`.
- Preserve existing domain spelling where it is already serialized or widely used, even if imperfect, for example `Meele`.

## Member Order

Prefer this order when creating or substantially editing a class:

1. Nested types.
2. Constants and static fields.
3. Events.
4. Serialized fields.
5. Public properties.
6. Private fields.
7. Injection methods / constructors.
8. Unity lifecycle methods.
9. Public methods.
10. Private/protected helpers.

Existing files do not always follow this perfectly. Improve nearby code when it is cheap, but do not reformat whole files just for style.

## Unity Patterns

- Cache local components in `Awake`.
- Use injected VContainer dependencies in `Start`, later lifecycle methods, or inside `[Inject] Construct(...)` after assignment.
- Subscribe/unsubscribe event pairs should be easy to see. Prefer named handler methods over anonymous lambdas when unsubscribe will be needed.
- Use `OnDestroy` for subscriptions made in `Start`; use `OnDisable` for subscriptions made in `OnEnable`.
- Use `[RequireComponent]` for mandatory local components when adding new MonoBehaviours.
- Avoid `FindObjectOfType` and new singleton access in gameplay/UI code. Prefer serialized references for local scene wiring and VContainer for scene services.
- `GameInput.Instance` still exists and is used. Do not expand singleton usage to more systems unless there is a specific reason.

## VContainer Style

- Use method injection for MonoBehaviours:

```csharp
private WavesManager _wavesManager;

[Inject]
public void Construct(WavesManager wavesManager)
{
    _wavesManager = wavesManager;
}
```

- Register scene-owned MonoBehaviours with `RegisterComponentInHierarchy<T>()` in `SceneLifetimeScope`.
- Do not register MonoBehaviours with `Register<T>(Lifetime.Singleton)`.
- If a `NightPool.Spawn` result needs injected dependencies, inject it manually through `IObjectResolver.InjectGameObject`.
- Read `Assets/Docs/VContainer_Guide_Undead_Harvest.md` before non-trivial DI changes.

## Coroutines And Async

- This project currently uses Unity coroutines and DOTween, not UniTask.
- Do not introduce UniTask or `System.Threading.Tasks` unless the user explicitly asks or there is a clear project decision.
- Coroutine methods should return `IEnumerator` and be named for the action they perform, for example `RunWave` or `SpawnEnemyRoutine`.
- Stop coroutines you own when their lifecycle ends.

## ScriptableObjects

- Use ScriptableObjects for configuration: weapons, passive items, waves, enemy stats, player stats, status effects, rarity config, item pools.
- Keep SO fields inspector-friendly. Existing SOs often use public fields; follow nearby style in the same SO family.
- Runtime identity/state belongs in runtime classes such as `PassiveItemInstance`, not in shared SO assets.
- For item descriptions, keep template logic in partial description files and formatting helpers rather than duplicating UI strings in every UI class.

## Formatting Preferences

- Use braces for `if`, `for`, `foreach`, `while`, and `switch` blocks when adding new code.
- Use early returns for guard clauses.
- Keep no more than one blank line between members.
- Do not mix unrelated refactors with feature fixes.
- Remove obsolete comments when touching a block, but do not delete commented-out experimental code wholesale unless the task is cleanup.
- Avoid magic numbers in new logic. Use named serialized fields or constants when values affect gameplay tuning.

## Text And UI

- Existing UI uses TextMeshPro (`TextMeshProUGUI`), UGUI `Button`, `Image`, `Slider`, `Toggle`, and rich text tags.
- Preserve TextMeshPro sprite tags and color tags in item descriptions.
- UI scripts commonly wire serialized fields in the inspector and subscribe to manager events through VContainer-injected services.

## Things To Be Careful With

- Scene and prefab references can break if class names, file names, serialized field names, or component types are renamed.
- NightPool object lifecycle differs from normal instantiate/destroy. Reset pooled runtime state in `OnSpawn`, `OnEnable`, or explicit reset methods.
- `Time.timeScale = 0` affects gameplay coroutines using `WaitForSeconds`.
- Enemy and weapon spawning can require manual DI injection.
- Build settings scene names must match `Loader.Scene` enum names.

