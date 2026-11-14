# Фінальний звіт: Стандартизація UML-діаграм, інкапсуляція та оновлення курсової роботи

**Проєкт:** Undead Harvest  
**Дата оновлення:** 08.11.2026  
**Перевірено та верифіковано:** Усі 31 діаграма класів (включаючи нову легенду `00_legend`, 23 компонентні діаграми, 4 діаграми інвентарю `class_inventory_*`, 3 scope-діаграми `*_scope` та оглядову `00_overview`), 4 діаграми прецедентів/послідовностей, реальний C# код у `Assets/Scripts/` та документ курсового проєкту `Tsybrivskyi_course.odt` / `Tsybrivskyi_course_updated.odt`.

---

## 1. Вирішення зауважень викладача та стандартизація UML (ISO/IEC 19505 / UML 2.5.1)

### 1.1. Інкапсуляція та відображення полів даних
*Головне зауваження викладача: «Були питання до інкапсуляції, класи виглядали як без полів або з відкритими полями».*

**Що виправлено за стандартом:**
1. **Повний набір інкапсульованих полів:**
   - У всіх ключових класах додано закриті поля (`-` private), які інкапсулюють стан об'єкта та зберігають посилання на залежності.
   - Відкритий інтерфейс (`+` public) суворо обмежений властивостями з обмеженим доступом (`{get;}` або `{get; private set;}`) та методами.
   - Приклади впровадження:
     * `Player`: додано закриті поля `- _sprite`, `- _baseStats`, `- _effects`, `- _rigidbody`, `- _wavesManager`, `- _gameInput`, `- _soundManager`, `- _movement`, `- _level`, `- _health`. Відкритий доступ надається через властивості `+ Stats {get; private set;}`, `+ Movement {get;}`, `+ Level {get;}`, `+ Health {get;}`, `+ HealthSystem {get;}`.
     * `PlayerHealthSystem`: додано приватні поля `- _playerStats`, `- _healthRegen`, `- _regenAccumulator`, `- _isInitialized` та властивість `+ HealthSystem {get; private set;}`.
     * `PlayerLevelSystem`: додано приватні поля `- _playerStats`, `- _levelSystem`, `- _soundManager` та властивість `+ LevelSystem {get;}`.
     * `HealthSystem` та `LevelSystem`: реактивні властивості повністю інкапсульовані — внутрішні змінні є закритими `ReactiveProperty<T>`, а назовні надаються як `ReadOnlyReactiveProperty<T> {get;}`.
     * `StatusEffectsManager` та `StatusEffectInstance`: додано закриті поля `- _enemy`, `- _activeEffects`, `- _stacks`, `- _tickTimer`.
     * `EquipmentManager`: додано закриті поля `- _weapons`, `- _playerStats`, `- _startingItems`, `- _startingWeapons`.
     * `PauseManager`: додано закрите поле `- _pauseCount : int` та публічні інспектори `+ PauseCount {get;}`, `+ IsPaused {get;}`.
2. **Виправлення неконфігураційних полів з порушенням інкапсуляції:**
   - `Enemy.Rigidbody`: раніше позначався як публічне поле `+ Rigidbody : Rigidbody2D`. У коді поле є закритим `private Rigidbody2D _rigidbody`, а доступ для нащадків надається через захищену властивість `protected Rigidbody2D Rigidbody => _rigidbody`. У діаграмі виправлено на `- _rigidbody : Rigidbody2D` та `# Rigidbody : Rigidbody2D {get;}`.
   - `Weapon.OriginalLocalPosition`: виправлено модифікатор видимості з `+` на `# OriginalLocalPosition : Vector3` (у коді поле захищене для спадкоємців `protected Vector3`).
   - `InventorySlot`: додано захищені посилання `# ItemImage : Image`, `# SlotOutline : Outline`, а зв'язок контролера винесено у закрите поле `- _itemInfoController`.
   - `WaveEntry`: усунуто дублювання поля `MaxEnemiesInScene` та зворотно-сумісної авто-властивості `maxEnemiesInScene` (залишено єдине канонічне поле `+ MaxEnemiesInScene : int = 30`).
   - `ItemInfoUI.OnClose`: позначено як `+ <<event>> OnClose : Action` (делегат події).

---

### 1.2. Стандартизація зв'язків між класами (стрілки та семантика)

| Тип зв'язку | Синтаксис | Де застосовано та чому |
|---|---|---|
| **Композиція** | `*--` | Сильний нерозривний зв'язок «частина-ціле», де життєвий цикл залежного об'єкта підпорядкований контейнеру:<br>• `Player *-- PlayerStats`, `Player *-- PlayerMovement`, `Player *-- PlayerHealthSystem`, `Player *-- PlayerLevelSystem`<br>• `PlayerHealthSystem *-- HealthSystem`<br>• `PlayerLevelSystem *-- LevelSystem`<br>• `PlayerStats *-- Stat`<br>• `StatusEffectsManager *-- StatusEffectInstance`<br>• `StatusEffectInstance *-- EffectStack`<br>• `WavesManager *-- WaveConfigSO`, `WaveConfigSO *-- WaveEntry`<br>• `ItemsInventoryUI *-- ItemInventorySlot` |
| **Агрегація** | `o--` | Слабкий зв'язок «частина-ціле», де складові існують незалежно від контейнера:<br>• `EnemyRegistry o-- Enemy`<br>• `Stat o-- StatModifier` |
| **Двонапрямлена асоціація** | `<-->` | Явне позначення взаємних перехресних посилань (усунуто невизначені лінії без стрілок `--`):<br>• `Enemy <--> StatusEffectsManager : взаємне посилання`<br>• `LevelUpPanelsController <--> LevelUpWindowUI`<br>• `InventorySlot <--> ItemInfoController` |
| **Створення** | `..> : «create»` | Явне інстанціювання або спавн екземплярів (new / пул об'єктів):<br>• `Player ..> PlayerStats/PlayerMovement/PlayerHealthSystem/PlayerLevelSystem : «create»`<br>• `PlayerHealthSystem ..> HealthSystem : «create»`<br>• `PlayerLevelSystem ..> LevelSystem : «create»`<br>• `PlayerStats ..> Stat : «create»`<br>• `EquipmentManager ..> PassiveItemInstance : «create»`<br>• `RangeWeaponSO ..> RangeWeaponContext : «create»`<br>• `EnemySpawner ..> Enemy : «create»`<br>• `SingleShotBehavior/ShotgunBehavior/BurstShotBehavior ..> PlayerBullet : «create»`<br>• `BossEnemy / RangerEnemy ..> Bullet : «create»`<br>• `ItemsInventoryUI ..> ItemInventorySlot : «create»` |
| **Залежність інтерфейсу** | `..>` | `IDamageable ..> HealthSystem : +HealthSystem {get;}` — інтерфейс не зберігає посилання як поле, а оголошує контракт доступу до системи здоров'я. |

---

### 1.3. Єдина візуальна легенда (`00_legend.puml`)
Створено та скомпільовано окрему діаграму-легенду `00_legend.puml`, що містить:
1. Зразки всіх типів зв'язків: композиція (`*--`), агрегація (`o--`), спрямована асоціація (`-->`), двонапрямлена асоціація (`<-->`), залежність (`..>`), створення (`..> : «create»`), спадкування (`<|--`), реалізація інтерфейсу (`<|..`).
2. Стандартні позначення видимості: `+` public, `-` private, `#` protected, `~` internal.
3. Позначення властивостей: `{get;}`, `{get; private set;}`.
4. Колірна диференціація:
   - **Білий фон** — власні класи поточної діаграми;
   - **Світло-сірий фон (`#EEEEEE`)** — класи інших підсистем (`пакет::Клас`) для наочності міжпакетних зв'язків;
   - **Блакитний фон (`#DDEBF7`, стереотип `«library»`)** — класи рушія Unity (MonoBehaviour, ScriptableObject) та зовнішніх бібліотек.

---

## 2. Оновлення курсового звіту (`Tsybrivskyi_course.odt`)

1. **Оновлення вбудованих рисунків:**
   - Усі 26 рисунків (Рис. 1 — прецеденти, Рис. 2 — огляд, Рис. 3–23 — діаграми класів за підсистемами, Рис. 24–26 — діаграми послідовностей) оновлено у високій роздільній здатності (DPI 200).
   - Оновлені діаграми містять усі додані приватні поля, нові типи зв'язків, стереотипи `«create»` та уніфіковане колірне оформлення.
2. **Оновлення тексту розділу «Діаграма класів»:**
   - Додано вичерпний теоретичний опис специфікації UML 2.5.1, правил читання зв'язків, принципу інкапсуляції, модифікаторів видимості та семантики стрілок (композиція, агрегація, створення, асоціації).
   - Актуалізовано кількість унікальних оголошень типів: **145** (із включенням `PlayerInputActions`).
3. **Валідація формату ODF:**
   - Документ успішно перепаковано за стандартом OpenDocument (незжатий заголовок `mimetype`, валідний `content.xml`).
   - Перевірено парсером ElementTree — структура XML бездоганна, документ відкривається без будь-яких помилок чи пошкоджень.

---

## 3. Сформовані файли та архіви

1. **`Tsybrivskyi_course.odt`** та **`Tsybrivskyi_course_updated.odt`** — фінальні документи звіту з оновленими рисунками та текстом.
2. **`diagrams_new.zip`** — оновлений архів, що містить цей фінальний `REPORT.md` та `diagrams_checked.zip`.
3. **`diagrams_checked.zip`** — повний комплект оновлених файлів:
   - `puml/` — 31 файл вихідного коду PlantUML (з легендою `00_legend.puml`);
   - `png/` — 31 відрендерений PNG файл високої роздільної здатності;
   - `svg/` — векторні діаграми.
4. **`diagrams_updated.zip`** та **`uml_usecase_and_sequence_updated.zip`** — синхронізовані системні архіви для звітності.
