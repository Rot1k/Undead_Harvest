using System;
using UnityEngine;

public class LevelSystem
{
    public event EventHandler OnLevelChanged;
    public event EventHandler OnExpChanged;
    public event EventHandler OnSkillPointUsed;

    public int Level { get; private set; }
    public int SkillPoints { get; private set; }
    private float _currentExp;
    private float _expToNextLevel;

    private readonly float _baseExp = 100f;
    private readonly float _growthFactor = 1.15f;

    public LevelSystem()
    {
        Level = 0;
        _currentExp = 0f;
        RecalculateExpToNextLevel();
    }

    public void AddExp(float exp)
    {
        _currentExp += exp;

        while (_currentExp >= _expToNextLevel)
        {
            _currentExp -= _expToNextLevel;
            Level++;
            SkillPoints++;
            RecalculateExpToNextLevel();
            OnLevelChanged?.Invoke(this, EventArgs.Empty);
        }

        OnExpChanged?.Invoke(this, EventArgs.Empty);
        Debug.Log($"Level: {Level}, Exp: {_currentExp}/{_expToNextLevel}");
    }

    public void UseSkillPoint(int amount)
    {
        if (SkillPoints >= amount)
        {
            SkillPoints -= amount;
            OnSkillPointUsed?.Invoke(this, EventArgs.Empty);
        }
    }

    public float GetExpPercent()
    {
        return _currentExp / _expToNextLevel;
    }

    private void RecalculateExpToNextLevel()
    {
        _expToNextLevel = _baseExp * Mathf.Pow(_growthFactor, Level);
    }
}
