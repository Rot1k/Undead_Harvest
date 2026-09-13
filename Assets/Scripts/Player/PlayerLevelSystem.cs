using System;
using UnityEngine;

public class PlayerLevelSystem
{
    private PlayerStats _playerStats;
    private LevelSystem _levelSystem;
    private SoundManager _soundManager;

    public LevelSystem LevelSystem => _levelSystem ?? 
        throw new InvalidOperationException("PlayerLevelSystem is not initialized. Call StartGame(PlayerStats) first.");

    public PlayerLevelSystem(PlayerStats playerStats, SoundManager soundManager)
    {
        _playerStats = playerStats;
        _soundManager = soundManager;
        _levelSystem = new LevelSystem();
        _levelSystem.OnLevelChanged += HandleLevelChanged;
    }

    public void SetSoundManager(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    public void Dispose()
    {
        if (_levelSystem != null)
        {
            _levelSystem.OnLevelChanged -= HandleLevelChanged;
        }
    }

    public void AddExp(float baseExp)
    {
        if (_levelSystem == null || _playerStats == null)
        {
            throw new InvalidOperationException("PlayerLevelSystem is not initialized. Call StartGame(PlayerStats) first.");
        }

        float multiplier = _playerStats.Get(StatType.ExperienceMultiplier);
        _levelSystem.AddExp(baseExp * multiplier);
    }

    private void HandleLevelChanged(object sender, EventArgs e)
    {
        _soundManager?.PlaySound(SoundType.LEVELUP);
    }
}
