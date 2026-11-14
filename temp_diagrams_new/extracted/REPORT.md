# Фінальний checkup UML-діаграм — Undead Harvest

Перевірено: 30 діаграм з архіву (23 тематичні, 4 inventory-аркуші, 3 scope-діаграми + main_menu_scope, оглядова) і текст курсової.
**Не перевірено** (їх немає в архіві): діаграма прецедентів (Рис. 1) і три діаграми послідовностей (Рис. 24–26).
**Обмеження:** код проєкту (Assets/Scripts) і Unity CLI у моєму середовищі недоступні, тому звірки з реальним кодом немає.
Усе нижче — перевірка діаграм між собою, з текстом курсової та з UML 2.5.1.

## 1. Що виправлено в діаграмах
| Діаграма | Зміна | Чому |
|---|---|---|
| 00_legend (нова) | Єдина легенда: асоціація / залежність / «create» / успадкування / реалізація / агрегація / композиція, видимість +,-,#,~, кольори | Викладач по-іншому читав стрілки; у тексті курсової не було опису агрегації, композиції та кольорів |
| core_boot (Рис. 3) | 15 класів інших діаграм (Player, WavesManager, 7 станів…) позначені як чужі (`пакет::Клас`, сірі); додано відсутній PauseManager; кольори уніфіковано | Раніше діаграма показувала ці класи як свої: хибне уявлення про те, хто чим володіє; в оглядовій схемі заявлено 8 класів, а фактично було 22 |
| scope-діаграми (4 шт.) | Уніфіковано кольори (білий — власний клас, сірий — чужа діаграма, блакитний — бібліотека) | Однаковий вигляд з рештою діаграм |
| input (Рис. 6) | Згенеровані класи Input System скорочено до ключових членів + «… (усього N)»; прибрано службовий префікс `@` | Було 22 см заввишки, ~5 pt на друку |
| effects (Рис. 12) | `StatusEffectsManager o-- StatusEffectInstance` → композиція `*--` | У тексті курсової: менеджер сам створює екземпляр ефекту й знищує його |
| waves (Рис. 11) | Додано `EnemySpawner ..> Enemy : «create»` | У тексті курсової: спавнер бере ворога з пулу й ініціалізує |
| items_rarity (Рис. 17) | У інтерфейсі IHaveRarity `{get; private set;}` → `{get;}` | Інтерфейс не може оголошувати приватний сеттер |
| class_inventory_gameplay | `State` → `MeeleWeaponState`; додано PlayerInputActions; лічильник 50 → 51 | У inventory тип `State` повторювався (клас GameFlow та enum зброї), а PlayerInputActions бракував, хоча в примітці заявлено, що він включений |

Після цих змін у курсовій потрібно замінити «144 унікальні оголошення» на **145**.

## 2. Читабельність (головна проблема)
Усі зображення в ODT вставлені на 17 см. Ефективний розмір шрифту на друку: більшість діаграм має 4–6 pt (мінімум для рисунків — 7–8 pt).
Перегенерація з іншим напрямком/відступами **не допомогла** (перевірено на всіх 30). Заміна «блоків вхідних залежностей» на нотатки теж зробила діаграми ширшими. Тому макети залишено, а проблема вирішується розміром сторінки:

| Рішення | Результат |
|---|---|
| Рисунки на альбомній сторінці (ширина 25,7 см) | 28 з 31 діаграм ≥ 7 pt (найслабші: class_inventory_ui 7,1; 00_overview і class_inventory_configuration 7,8) |
| Лишаються < 7 pt навіть так | class_inventory_gameplay (4,4), class_inventory_systems (6,6), core_boot (6,8) — потрібно розбити на 2 частини або подати як таблицю в додатку |

Повна таблиця: `metrics` у розділі 4.

## 3. Потребує вашого рішення / перевірки по коду (не змінював без джерела)
**Стрілки**
- Три асоціації без стрілки (двонапрямлені): `Enemy -- StatusEffectsManager` (enemies), `LevelUpPanelsController -- LevelUpWindowUI` (ui_menu), `InventorySlot -- ItemInfoController` (ui_inventory). Викладач може прочитати їх як «невідомо хто кого використовує». Якщо навігація в один бік — змінити на `-->`.
- `IDamageable --> HealthSystem` (player_systems): інтерфейс не зберігає посилань; перевірити, чи не помилка генерації.
- Агрегація/композиція: `PlayerStats o-- Stat` (Stat, ймовірно, створюється всередині), `StatusEffectInstance o-- EffectStack` (структура за значенням), `ItemsInventoryUI o-- ItemInventorySlot` (UI сам створює слоти) — імовірно мають бути `*--`.

**Інкапсуляція**
- У жодній діаграмі немає приватних членів (`-`): показано лише public/protected. Асоціації видно, а приватні поля, у яких зберігаються посилання, — ні. Для прозорості додайте ключові приватні поля (напр. `- _stats : PlayerStats`), тоді стрілка отримає ім'я ролі.
- Публічні змінювані поля у не-конфігураційних класах: `Enemy.Rigidbody`, `Weapon.OriginalLocalPosition`, `InventorySlot.ItemImage/SlotOutline`, `PassiveItemInstance.RuntimeData : object`, `RangeWeaponContext.Damage/FireRate/BulletSpeed`.
- `ItemInfoUI.OnClose : Action` — публічне поле-делегат без `event`; потенційно порушення інкапсуляції (решта подій оголошені як `<<event>>`).
- `WaveEntry` містить і поле `MaxEnemiesInScene`, і властивість `maxEnemiesInScene` — схоже на дублювання.
- У тексті курсової: `IReadOnlyReactiveProperty<T>`; на діаграмах: `ReadOnlyReactiveProperty<T>` — узгодити.

**Хто що створює (документовано лише частково)**
Підтверджено текстом курсової: Unity створює MonoBehaviour зі сцени, VContainer — зареєстровані сервіси/стани/Bootstrap, EnemySpawner — ворогів, StatusEffectsManager — екземпляри ефектів.
Кандидати за сигнатурами конструкторів — **треба підтвердити кодом** (додати `..> : «create»`):

| Хто створює | Що створює | Ознака |
|---|---|---|
| Player | PlayerStats, PlayerMovement, PlayerHealthSystem, PlayerLevelSystem | звичайні C#-класи з конструкторами, не в списку реєстрації |
| PlayerHealthSystem / PlayerLevelSystem | HealthSystem / LevelSystem | конструктори HealthSystem(int), LevelSystem() |
| PlayerStats | Stat | Stat(StatType, float) |
| StatusEffectInstance | EffectStack | EffectStack(float) |
| EquipmentManager | PassiveItemInstance | PassiveItemInstance(PassiveItemSO) |
| RangeWeaponSO | RangeWeaponContext | метод GetContext() (зараз показано як звичайна залежність) |
| IWeaponBehavior-реалізації | PlayerBullet | пул снарядів |
| BossEnemy / RangerEnemy | EnemyBullet | пул снарядів |
| ItemsInventoryUI | ItemInventorySlot | SpawnItemUI |

Команда для перевірки в проєкті:
`grep -rnE "new (PlayerStats|PlayerMovement|PlayerHealthSystem|PlayerLevelSystem|HealthSystem|LevelSystem|Stat|EffectStack|PassiveItemInstance|RangeWeaponContext)\(|Instantiate|Spawn\(|Get<|Rent" Assets/Scripts`

## 4. Текст курсової
- Блок «Діаграми класів»: додати опис агрегації, композиції, «create», кольорів і посилання на легенду (нова перша діаграма).
- Підпис про «144 унікальні оголошення» → 145.
- Вставлена в ODT `core_boot` — стара версія (1702 px), у архіві — оновлена; scope-діаграм (4 шт.) в ODT немає.
- Папки `png_all`, `png_utf8`, `png_updated` в архіві — ідентичні копії `png`, а в `png_updated` бракує частини файлів. Залиште один набір (`png/`).

### metrics (ефективний шрифт класів, pt)
| Діаграма | Портрет 17 см | Альбом 25,7 см |
|---|---|---|
| 00_legend | 7.6 | 11.4 |
| 00_overview | 5.1 | 7.8 |
| class_inventory_configuration | 5.2 | 7.8 |
| class_inventory_gameplay | 4.7 | 4.4 |
| class_inventory_systems | 5.3 | 6.6 |
| class_inventory_ui | 4.7 | 7.1 |
| core_boot | 7.8 | 6.8 |
| core_services | 6.1 | 9.2 |
| effects | 6.0 | 8.0 |
| enemies | 6.0 | 9.1 |
| flow | 5.1 | 7.7 |
| global_lifetime_scopes | 10.1 | 15.2 |
| input | 11.1 | 8.3 |
| items | 7.6 | 11.5 |
| items_rarity | 8.7 | 9.0 |
| main_menu_scope | 13.1 | 13.2 |
| player | 6.6 | 9.8 |
| player_systems | 6.7 | 10.1 |
| projectiles | 6.2 | 9.4 |
| scene_lifetime_scope | 5.4 | 8.2 |
| scene_registered_services | 7.7 | 11.7 |
| stats | 6.7 | 8.5 |
| tests | 11.9 | 18.0 |
| ui_hud | 8.4 | 7.5 |
| ui_inventory | 7.1 | 10.8 |
| ui_menu | 5.6 | 8.5 |
| ui_results | 9.6 | 12.1 |
| ui_shop | 6.7 | 10.1 |
| waves | 9.2 | 8.8 |
| weapons | 6.7 | 10.1 |
| weapons_data | 8.1 | 12.2 |
