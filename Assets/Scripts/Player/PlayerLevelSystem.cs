using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    public LevelSystem LevelSystem { get; private set; }
    [SerializeField] private PlayerStats _playerStats;

    private void Awake()
    {
        LevelSystem = new LevelSystem();
    }

    public void AddExp(float baseExp)
    {
        float multiplier = _playerStats != null ? _playerStats.Get(StatType.ExperienceMultiplier) : 1f;
        LevelSystem.AddExp(baseExp * multiplier);
    }
}
