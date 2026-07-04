using System;
using UnityEngine;
using R3;
public class LevelSystem
{
    public event EventHandler OnLevelChanged;
    public event EventHandler OnExpChanged;
    public event EventHandler OnSkillPointUsed;

    private ReactiveProperty<int> _level;
    private ReactiveProperty<float> _currentExp;
    private ReactiveProperty<float> _expToNextLevel;

    public ReadOnlyReactiveProperty<int> Level => _level;
    public ReadOnlyReactiveProperty<float> CurrentExp => _currentExp;
    public ReadOnlyReactiveProperty<float> ExpToNextLevel => _expToNextLevel;

    public int SkillPoints { get; private set; }

    private readonly float _baseExp = 100f;
    private readonly float _growthFactor = 1.15f;

    public LevelSystem()
    {
        _level = new ReactiveProperty<int>(0);
        _currentExp = new ReactiveProperty<float>(0f);
        _expToNextLevel = new ReactiveProperty<float>(_baseExp);
    }

    public void AddExp(float exp)
    {
        _currentExp.Value += exp;

        while (_currentExp.Value >= _expToNextLevel.Value)
        {
            _currentExp.Value -= _expToNextLevel.Value;
            _level.Value++;
            SkillPoints++;
            RecalculateExpToNextLevel();
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }

        OnExpChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Level: {_level.Value}, Exp: {_currentExp.Value}/{_expToNextLevel.Value}");
    }

    public void UseSkillPoint(int amount)
    {
        if (SkillPoints >= amount)
        {
            SkillPoints -= amount;
            OnSkillPointUsed?.Invoke(this, EventArgs.Empty);
        }
    }
    private void RecalculateExpToNextLevel()
    {
        _expToNextLevel.Value = _baseExp * Mathf.Pow(_growthFactor, _level.Value);
    }
}
