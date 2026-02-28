using System.Collections.Generic;
using UnityEngine;

public class StatusEffectInstance
{
    public StatusEffectSO EffectData { get; }
    public Enemy Target { get; }
    public bool IsFinished => _stacks.Count == 0;

    private readonly List<EffectStack> _stacks = new();
    private float _tickTimer;

    public StatusEffectInstance(StatusEffectSO effectData, Enemy target)
    {
        EffectData = effectData;
        Target = target;

        foreach (var modifier in EffectData.Modifiers)
        {
            modifier.OnApply(this, Target);
            Debug.Log(modifier);
        }
    }

    public void AddStack(float duration)
    {
        _stacks.Add(new EffectStack(duration));
    }

    public void Update(float deltaTime)
    {
        UpdateStacks(deltaTime);
        HandleTick(deltaTime);
    }

    private void UpdateStacks(float deltaTime)
    {
        for (int i = _stacks.Count - 1; i >= 0; i--)
        {
            _stacks[i].RemainingDuration -= deltaTime;

            if (_stacks[i].RemainingDuration <= 0)
                _stacks.RemoveAt(i);
        }

        if (_stacks.Count == 0)
        {
            foreach (var modifier in EffectData.Modifiers)
            {
                modifier.OnExpire(this, Target);
            }
        }
    }

    private void HandleTick(float deltaTime)
    {
        if (EffectData.TickInterval <= 0)
            return;

        _tickTimer += deltaTime;

        if (_tickTimer >= EffectData.TickInterval)
        {
            _tickTimer = 0f;

            int stackCount = _stacks.Count;

            if (stackCount > 0)
            {
                foreach (var modifier in EffectData.Modifiers)
                    modifier.OnTick(this, Target, stackCount);
            }
        }
    }
    public void ForceExpire()
    {
        foreach (var modifier in EffectData.Modifiers)
        {
            modifier.OnExpire(this, Target);
        }

        _stacks.Clear();
    }

    public void Refresh(float duration)
    {
        _stacks.Clear();
        _stacks.Add(new EffectStack(duration));
    }

    public int GetStackCount() => _stacks.Count;

}