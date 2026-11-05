# Diagram Mapping & Commit Impact Guide

This reference documents the 22 class diagrams, 3 sequence diagrams, and 1 use-case diagram in the Undead Harvest course project, along with instructions on which diagram to inspect and update when specific code components change in git.

---

## 1. Complete Diagram Table

| Figure | Diagram Name | Primary PUML File | Key Classes & Interfaces | Trigger when modified in git |
| :--- | :--- | :--- | :--- | :--- |
| **Рис. 1** | Use Case Diagram | `usecase.puml` | Actor `Гравець`, Main use-cases | Player features, UI flows, high-level mechanics |
| **Рис. 2** | Architecture Overview | `00_overview.puml` | Package groupings, class counts, layer coupling | Adding/removing classes across modules |
| **Рис. 3** | Core: Bootstrap & DI | `core_boot.puml` | `Bootstrap`, `SceneLifetimeScope`, `GlobalLifetimeScope`, `PauseManager`, `Loader` | `Assets/Scripts/Systems/DI Containers/`, `Bootstrap.cs`, `PauseManager.cs` |
| **Рис. 4** | Core: Services | `core_services.puml` | `WalletManager`, `ShopService`, `IShopService`, `SoundManager` | `Assets/Scripts/Systems/WalletManager.cs`, `ShopService.cs`, Audio managers |
| **Рис. 5** | Game Flow / FSM | `flow.puml` | `StateMachine`, `State`, `GameFlowState`, `WaveState`, `LevelUpState`, `ShopState`, `GameOverState`, `VictoryState` | `Assets/Scripts/GameFlow/GameFlowStates.cs`, `StateMachine.cs` |
| **Рис. 6** | Input System | `input.puml` | `InputManager`, `PlayerInputActions` wrapper | `Assets/Scripts/Input/` |
| **Рис. 7** | Player Components | `player.puml` | `Player`, `PlayerMovement`, `PlayerAnimation` | `Assets/Scripts/Player/Player.cs`, movement scripts |
| **Рис. 8** | Player Systems | `player_systems.puml` | `PlayerHealthSystem`, `HealthSystem`, `ItemMagnet`, `ItemCollector`, `CombatSystem`, `IDamageable` | `Assets/Scripts/Player/` health, magnet, collector, combat |
| **Рис. 9** | Stats & Modifiers | `stats.puml` | `PlayerStats`, `Stat`, `StatModifier`, `ModifierType`, `StatType`, `PlayerStatsSO` | `Assets/Scripts/Player/StatSystem/`, `PlayerStats.cs`, `StatModifier.cs` |
| **Рис. 10**| Enemies | `enemies.puml` | `Enemy`, `BossEnemy`, `DashEnemy`, `RangerEnemy`, `EnemyVisual`, `EnemyRegistry` | `Assets/Scripts/Enemies/`, `Enemy.cs` |
| **Рис. 11**| Waves & Spawning | `waves.puml` | `WavesManager`, `EnemySpawner`, `WaveConfigSO`, `WaveEntry`, `EnemyRegistry` | `Assets/Scripts/Systems/WavesManager.cs`, `EnemySpawner.cs`, `EnemyRegistry.cs` |
| **Рис. 12**| Status Effects | `effects.puml` | `StatusEffectsManager`, `StatusEffectSO`, `StatusEffectInstance`, `EffectStack` | `Assets/Scripts/Enemies/Status Effects/`, `StatusEffectsManager.cs` |
| **Рис. 13**| Weapons | `weapons.puml` | `Weapon`, `RangeWeapon`, `MeeleWeapon`, `WeaponsHolder`, `WeaponVisual` | `Assets/Scripts/Weapons/Weapon.cs`, `WeaponsHolder.cs` |
| **Рис. 14**| Weapon Data | `weapons_data.puml` | `WeaponSO`, `RangeWeaponSO`, `MeeleWeaponSO` | `Assets/Scriptable Objects/Scripts/WeaponSO.cs` |
| **Рис. 15**| Projectiles & Behavior | `projectiles.puml` | `Bullet`, `EnemyBullet`, `PlayerBullet`, `IWeaponBehavior`, `SingleShotBehavior`, `BurstShotBehavior`, `ShotgunBehavior` | `Assets/Scripts/Combat/Bullet.cs`, `IWeaponBehavior.cs` |
| **Рис. 16**| Items & Pool | `items.puml` | `InventoryItemSO`, `InventoryItemsPoolSO`, `PassiveItemSO`, `PassiveItemInstance`, `ModifierData`, `IDescribableItem` | `Assets/Scripts/Items/`, `PassiveItemSO.cs` |
| **Рис. 17**| Items Rarity & Equip | `items_rarity.puml` | `EquipmentManager`, `RarityConfigSO`, `IEquippable`, `IHaveRarity` | `Assets/Scripts/Systems/EquipmentManager.cs` |
| **Рис. 18**| UI: HUD | `ui_hud.puml` | `HUDUI`, `UIBootstrap`, `HealthBarUI`, `WalletUI`, `WaveTimerUI` | `Assets/Scripts/UI/` HUD components |
| **Рис. 19**| UI: Menus & Level Up | `ui_menu.puml` | `MainUI`, `SettingsUI`, `LevelUpWindowUI`, `TutorialUI` | `Assets/Scripts/UI/` Menus |
| **Рис. 20**| UI: Results & Stats | `ui_results.puml` | `WaveEndWindow`, `WinLoseWindowUI`, `StatsTableUI`, `StatUI`, `WeaponsStatisticsUI` | `Assets/Scripts/UI/StatUI.cs`, Result screens |
| **Рис. 21**| UI: Shop | `ui_shop.puml` | `ShopUI`, `BuyWindow`, `BuyWindowsHolder` | `Assets/Scripts/UI/ShopUI.cs` |
| **Рис. 22**| UI: Inventory | `ui_inventory.puml` | `ItemsInventoryUI`, `WeaponsInventoryUI`, `InventorySlot` | `Assets/Scripts/UI/` Inventory |
| **Рис. 23**| Unit Tests | `tests.puml` | `HealthSystemTests`, `PlayerStatsTests`, `StatusEffectsTests` | `Assets/Editor/` test scripts |
| **Рис. 24**| Sequence: Bootstrap & DI | `seq_1_start_DI.puml` | Engine, `SceneLifetimeScope`, `IContainerBuilder`, `Bootstrap`, `Player` | Changes to initialization sequence |
| **Рис. 25**| Sequence: Game Loop | `seq_2_state_loop.puml` | `StateMachine`, `WaveState`, `WavesManager`, `LevelUpState`, `ShopState`, `PauseManager` | FSM transitions, pause logic |
| **Рис. 26**| Sequence: Combat & Damage| `seq_3_combat.puml` | `RangeWeapon`, `EnemyRegistry`, `CombatSystem`, `PlayerBullet`, `Enemy`, `HealthSystem` | Combat pipeline, weapon targeting, damage |

---

## 2. Git Commit Analysis Rules

When inspecting a git commit or branch diff:
1. Run `git diff <commit> --name-only` or `git log -n <count> --stat`.
2. For each changed file, look up the corresponding diagram(s) using the table above.
3. Compare class signatures, members, and relationships:
   - Added/removed/renamed public methods or properties.
   - Added/removed constructor parameters or dependencies.
   - Removed or added interfaces/classes.
   - Reactive programming changes (e.g., event delegates migrated to R3 `Observable<T>`).
4. Apply minimal surgical edits to the matching `.puml` files.
5. Run the sync script `sync_diagrams.py` to regenerate PNGs and update the `.odt` report.
