using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class StatusEffectsManager : MonoBehaviour
{
    private Enemy _enemy;

    private Dictionary<StatusEffectSO, StatusEffectInstance> _activeEffects = new();

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        foreach (var pair in _activeEffects.ToList())
        {
            pair.Value.Update(dt);

            if (pair.Value.IsFinished)
                _activeEffects.Remove(pair.Key);
        }
    }

    public void ApplyEffect(StatusEffectSO effectSO)
    {
        if (_activeEffects.TryGetValue(effectSO, out var instance))
        {
            if (!effectSO.IsStackable)
            {
                instance.Refresh(effectSO.Duration);
                return;
            }

            if (instance.GetStackCount() >= effectSO.MaxStacks)
                return;

            instance.AddStack(effectSO.Duration);
        }
        else
        {
            var newInstance = new StatusEffectInstance(effectSO, _enemy);
            newInstance.AddStack(effectSO.Duration);
            _activeEffects.Add(effectSO, newInstance);
        }

        OnApplyEffectColorSetter(effectSO.Color);
    }
    public void ClearAllEffects()
    {
        foreach (var instance in _activeEffects.Values)
        {
            instance.ForceExpire();
        }

        _activeEffects.Clear();
    }
    private void OnApplyEffectColorSetter(Color color)
    {
        if (TryGetComponent<EnemyVisual>(out var visual))
        {
            StartCoroutine(visual.SetTempColor(color, 0.2f));
        }
    }
}