using UnityEngine;
using System.Collections.Generic;
using NTC.Pool;
using VContainer;
using VContainer.Unity;

public class WeaponsHolder : MonoBehaviour
{


    [SerializeField] private float _radius = 0.8f;
    [SerializeField] private float _radiusStep = 0.8f;

    private readonly Dictionary<int, GameObject> _spawnedWeapons = new();

    private WavesManager _wavesManager;
    private EquipmentManager _equipmentManager;
    private PlayerStats _playerStats;
    private PlayerMovement _playerMovement;

    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
    }
    public void Initialize(
        EquipmentManager equipmentManager,
        PlayerStats playerStats,
        WavesManager wavesManager,
        PlayerMovement playerMovement)
    {
        _equipmentManager = equipmentManager;
        _playerStats = playerStats;
        _wavesManager = wavesManager;
        _playerMovement = playerMovement;

        _equipmentManager.OnWeaponEquipped += SpawnWeapon;
        _equipmentManager.OnWeaponUnequipped += DespawnWeapon;

        _wavesManager.OnWaveCompleted += ResetWeapons;

        // Spawn any weapons that were equipped before this holder initialized
        for (int i = 0; i < _equipmentManager.MaxWeapons; i++)
        {
            var w = _equipmentManager.GetWeapon(i);
            if (w != null)
            {
                SpawnWeapon(i, w);
            }
        }

        // Final arrange to position spawned weapons
        Arrange();
    }
    public void Dispose()
    {
        _equipmentManager.OnWeaponEquipped -= SpawnWeapon;
        _equipmentManager.OnWeaponUnequipped -= DespawnWeapon;

        _wavesManager.OnWaveCompleted -= ResetWeapons;
    }

    private void SpawnWeapon(int slot, WeaponSO weapon)
    {
        if (weapon == null) return;

        // Despawn old if exists
        if (_spawnedWeapons.TryGetValue(slot, out GameObject existing))
        {
            NightPool.Despawn(existing);
            _spawnedWeapons.Remove(slot);
        }

        GameObject weaponGO = NightPool.Spawn(weapon.Prefab, transform);
        _objectResolver.InjectGameObject(weaponGO);
        _spawnedWeapons[slot] = weaponGO;

        if (weaponGO.TryGetComponent<Weapon>(out var weaponComponent))
        {
            weaponComponent.Initialize(_playerStats, _playerMovement, weapon);
            weaponComponent.UpdateStartPosition(weaponComponent.transform.localPosition);
        }
        Arrange();
    }

    private void DespawnWeapon(int slot, WeaponSO weapon)
    {
        if (_spawnedWeapons.TryGetValue(slot, out GameObject instance))
        {
            NightPool.Despawn(instance);
            _spawnedWeapons.Remove(slot);
        }
        Arrange();
    }

    private void Arrange()
    {
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        ArrangeAroundCircle(children);
    }

    private void ArrangeAroundCircle(Transform[] objects)
    {
        int totalCount = objects.Length;
        if (totalCount == 0)
        {
            return;
        }

        int maxPerCircle = 8;
        int circleIndex = 0;
        int placed = 0;

        while (placed < totalCount)
        {
            int remaining = totalCount - placed;
            int countThisCircle = Mathf.Min(remaining, maxPerCircle);

            List<float> angles = BuildVerticalSymmetricAngles(countThisCircle);
            float currentRadius = _radius + circleIndex * _radiusStep;

            for (int i = 0; i < countThisCircle; i++)
            {
                float rad = angles[i] * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * currentRadius;
                Transform obj = objects[placed + i];
                obj.position = transform.position + (Vector3)pos;
                if (obj.TryGetComponent<Weapon>(out Weapon weapon))
                {
                    weapon.UpdateStartPosition(obj.localPosition);
                }
            }

            placed += countThisCircle;
            circleIndex++;
        }
    }

    private List<float> BuildVerticalSymmetricAngles(int count)
    {
        List<float> angles = new List<float>();

        if (count == 1)
        {
            angles.Add(0f);
            return angles;
        }

        if (count == 3)
        {
            angles.Add(0f);
            angles.Add(90f);
            angles.Add(180f);
            return angles;
        }

        float step = 360f / count;
        float offset = 90f - (step * (count - 1) / 2f);

        for (int i = 0; i < count; i++)
        {
            angles.Add(offset + i * step);
        }

        return angles;
    }
    private void ResetWeapons()
    {
        foreach (var weaponGO in _spawnedWeapons.Values)
        {
            if (weaponGO.TryGetComponent<Weapon>(out var weapon))
            {
                weapon.ResetState();
            }
        }
    }
}
