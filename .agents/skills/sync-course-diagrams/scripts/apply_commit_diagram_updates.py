# -*- coding: utf-8 -*-
import os

repo_dir = r'C:\Unity projects\Undead_Harvest'
scratch_dir = r'C:\Users\User\.gemini\antigravity\brain\cd411519-42dd-4a01-ae3e-915e7e9edda9\scratch\extracted'

# 1. Update stats.puml
def update_stats(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # In Stat class:
    # Remove: +  <<event>> OnValueChanged : Action<float>
    # Add: + CurrentValueStream : ReadOnlyReactiveProperty<float> <<get>>
    # Add: + Dispose() : void
    content = content.replace(
        'class Stat {\n  +  <<event>> OnValueChanged : Action<float>\n  + BaseValue : float <<get>>\n  + CurrentValue : float <<get>>\n  + Type : StatType\n  + Stat(StatType, float)\n  + SetBase(float) : void\n  + AddModifier(StatModifier) : void\n  + RemoveModifiersFromSource(Guid) : void\n}',
        'class Stat {\n  + BaseValue : float <<get>>\n  + CurrentValue : float <<get>>\n  + CurrentValueStream : ReadOnlyReactiveProperty<float> <<get>>\n  + Type : StatType\n  + Stat(StatType, float)\n  + SetBase(float) : void\n  + AddModifier(StatModifier) : void\n  + RemoveModifiersFromSource(Guid) : void\n  + Dispose() : void\n}'
    )

    # In PlayerStats class:
    # Update Observe return type to ReadOnlyReactiveProperty<float>
    content = content.replace(
        '+ Observe(StatType) : Observable<float>',
        '+ Observe(StatType) : ReadOnlyReactiveProperty<float>'
    )

    # Add IDisposable inheritance to Stat
    if 'IDisposable <|.. Stat' not in content:
        content = content.replace(
            'IDisposable <|.. PlayerStats',
            'IDisposable <|.. PlayerStats\nIDisposable <|.. Stat'
        )

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f'Updated {file_path}')

# 2. Update weapons.puml
def update_weapons(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Add CalculateDamage(float) : float to Weapon
    if '+ CalculateDamage(float) : float' not in content:
        content = content.replace(
            '+ HandleOnHit(Enemy) : void',
            '+ CalculateDamage(float) : float\n  + HandleOnHit(Enemy) : void'
        )

    # Add CombatSystem dependency
    if 'CombatSystem' not in content:
        content = content.replace(
            'class "waves::EnemyRegistry" as EnemyRegistry #EEEEEE',
            'class "waves::EnemyRegistry" as EnemyRegistry #EEEEEE\nclass "player_systems::CombatSystem" as CombatSystem <<static>> #EEEEEE'
        )
        content = content.replace(
            'Weapon --> EnemyRegistry',
            'Weapon --> EnemyRegistry\nWeapon ..> CombatSystem'
        )

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f'Updated {file_path}')

# 3. Update player_systems.puml
def update_player_systems(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    # Add overload in CombatSystem
    if 'CalculateDamage(PlayerStats, WeaponSO, float)' not in content:
        content = content.replace(
            '+ {static} CalculateDamage(PlayerStats, WeaponSO) : float',
            '+ {static} CalculateDamage(PlayerStats, WeaponSO) : float\n  + {static} CalculateDamage(PlayerStats, WeaponSO, float) : float'
        )

    with open(file_path, 'w', encoding='utf-8') as f:
        f.write(content)
    print(f'Updated {file_path}')

# 4. Update seq_3_combat.puml
seq_3_puml = """@startuml
skinparam dpi 220
skinparam maxMessageSize 110
skinparam ParticipantPadding 2
autonumber
skinparam style strictuml
skinparam sequenceMessageAlign center

participant ":RangeWeapon" as Weapon
participant "EnemyRegistry\\n<<service>>" as Registry
participant "CombatSystem\\n<<static>>" as Combat
participant ":PlayerStats" as Stats
participant ":IWeaponBehavior\\n<<strategy>>" as Behavior
participant ":PlayerBullet" as Bullet
participant ":Enemy" as Enemy
participant ":HealthSystem" as Health
participant ":StatusEffectsManager" as StatusMgr

loop FixedUpdate / Update
    Weapon -> Registry : GetAll() / FindNearestEnemy()
    Registry --> Weapon : nearestEnemy
    opt new candidate closer AND current target still valid
        note right of Weapon : _targetSwitchDelay (0.15s)\\ncandidate must hold lead\\nbefore becoming new target
    end
end

opt CanShoot() == true
    Weapon -> Behavior : Shoot(this, firePoint, context, dir)
    activate Behavior
    
    Behavior -> Weapon : CalculateDamage(bulletBaseDamage)
    activate Weapon
    Weapon -> Combat : CalculateDamage(playerStats, weaponSO, bulletBaseDamage)
    activate Combat
    Combat -> Stats : Get(StatType.AttackDamage, CritChance...)
    Stats --> Combat : statValues
    Combat --> Weapon : finalDamage
    deactivate Combat
    Weapon --> Behavior : finalDamage
    deactivate Weapon

    Behavior -> Bullet : Spawn & Setup(dir, finalDamage, speed, this)
    activate Bullet
    deactivate Behavior

    ...Політ снаряда...
    Bullet -> Enemy : OnTriggerEnter2D(collision)
    Bullet -> Enemy : OnHit(IDamageable)

    Bullet -> Health : Damage(damageAmount)
    activate Health
    Health -> Health : Health.Value -= damageAmount
    opt Health <= 0
        Health --> Enemy : OnDead
        Enemy -> Registry : Unregister(this)
    end
    deactivate Health

    Bullet -> Weapon : HandleOnHit(enemy)
    Weapon -> StatusMgr : ApplyEffect(statusEffectSO)
    activate StatusMgr
    StatusMgr -> StatusMgr : AddStack() / Refresh()
    deactivate StatusMgr

    Bullet -> Bullet : Despawn()
    deactivate Bullet
end
@enduml
"""

for base in [os.path.join(repo_dir, "Diagrams", "Class"), os.path.join(scratch_dir, "diagrams", "puml")]:
    update_stats(os.path.join(base, "stats.puml"))
    update_weapons(os.path.join(base, "weapons.puml"))
    update_player_systems(os.path.join(base, "player_systems.puml"))

for base in [os.path.join(repo_dir, "Diagrams", "Sequence_UseCase"), os.path.join(scratch_dir, "usecase_seq", "puml")]:
    target = os.path.join(base, "seq_3_combat.puml")
    with open(target, 'w', encoding='utf-8') as f:
        f.write(seq_3_puml)
    print(f'Updated {target}')

print("All diagram PUML files successfully updated!")
