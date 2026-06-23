using UnityEngine;
using VContainer;

public class PlayerLevelSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats _playerStats;

    private LevelSystem _levelSystem;
    private SoundManager _soundManager;

    public LevelSystem LevelSystem
    {
        get
        {
            EnsureLevelSystem();
            return _levelSystem;
        }
    }

    [Inject]
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
        _levelSystem?.SetSoundManager(_soundManager);
    }

    private void Awake()
    {
        EnsureLevelSystem();
    }

    public void AddExp(float baseExp)
    {
        float multiplier = _playerStats != null ? _playerStats.Get(StatType.ExperienceMultiplier) : 1f;
        LevelSystem.AddExp(baseExp * multiplier);
    }

    private void EnsureLevelSystem()
    {
        if (_levelSystem != null)
        {
            return;
        }

        _levelSystem = new LevelSystem();
        _levelSystem.SetSoundManager(_soundManager);
    }
}
