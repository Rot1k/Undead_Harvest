using System;
using UnityEngine;
using VContainer;

public class PlayerLevelSystem : MonoBehaviour
{
    private PlayerStats _playerStats;
    private LevelSystem _levelSystem;
    private SoundManager _soundManager;

    public LevelSystem LevelSystem => _levelSystem ?? 
        throw new InvalidOperationException("PlayerLevelSystem is not initialized. Call StartGame(PlayerStats) first.");

    [Inject]
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    private void OnDestroy()
    {
        if (_levelSystem != null)
        {
            _levelSystem.OnLevelChanged -= HandleLevelChanged;
        }
    }

    public void Initialize(PlayerStats playerStats)
    {
        if (_levelSystem != null)
        {
            return;
        }

        _playerStats = playerStats;
        _levelSystem = new LevelSystem();
        _levelSystem.OnLevelChanged += HandleLevelChanged;
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
