using UnityEngine;
using System.Collections.Generic;
using NTC.Pool;

public class WeaponsHolder : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private float _radius = 0.8f;
    [SerializeField] private float _radiusStep = 0.8f;

    private readonly Dictionary<int, GameObject> _spawnedWeapons = new();

    private void Awake()
    {
        Arrange();
        EquipmentManager.Instance.OnWeaponEquipped += SpawnWeapon;
        EquipmentManager.Instance.OnWeaponUnequipped += DespawnWeapon;
        WavesManager.Instance.OnWaveCompleted += ResetWeapons;
    }
    private void OnDestroy()
    {
        if (EquipmentManager.Instance != null)
        {
            EquipmentManager.Instance.OnWeaponEquipped -= SpawnWeapon;
            EquipmentManager.Instance.OnWeaponUnequipped -= DespawnWeapon;
        }
    }

    private void SpawnWeapon(int slot, WeaponSO weapon)
    {
        if (weapon == null) return;

        // Despawn old if exists
        if (_spawnedWeapons.TryGetValue(slot, out var existing))
        {
            NightPool.Despawn(existing);
            _spawnedWeapons.Remove(slot);
        }

        GameObject weaponGO = NightPool.Spawn(weapon.Prefab, transform);
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
        if (_spawnedWeapons.TryGetValue(slot, out var instance))
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
            children[i] = transform.GetChild(i);

        ArrangeAroundCircle(children);
    }

    private void ArrangeAroundCircle(Transform[] objects)
    {
        int totalCount = objects.Length;
        if (totalCount == 0) return;

        int maxPerCircle = 8;
        int circleIndex = 0;
        int placed = 0;

        while (placed < totalCount)
        {
            int remaining = totalCount - placed;
            int countThisCircle = Mathf.Min(remaining, maxPerCircle);

            var angles = BuildVerticalSymmetricAngles(countThisCircle);
            float currentRadius = _radius + circleIndex * _radiusStep;

            for (int i = 0; i < countThisCircle; i++)
            {
                float rad = angles[i] * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * currentRadius;
                Transform obj = objects[placed + i];
                obj.position = transform.position + (Vector3)pos;
                if (obj.TryGetComponent<Weapon>(out var weapon))
                    weapon.UpdateStartPosition(obj.localPosition);
            }

            placed += countThisCircle;
            circleIndex++;
        }
    }

    private List<float> BuildVerticalSymmetricAngles(int count)
    {
        var angles = new List<float>();

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
            angles.Add(offset + i * step);

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