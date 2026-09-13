using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpPanelsController : MonoBehaviour
{
    [SerializeField] private LevelUpWindowUI _levelUpWindowUI;
    [SerializeField] private RarityConfigSO _rarityConfigSO;

    private PlayerStatsSO _playerStatsSO;
    private PlayerStats _playerStats;

    private LevelUpStatPanel[] _levelUpWindows;

    public void Initialize(Player player)
    {
        PlayerStats playerStats = player.Stats;
        PlayerStatsSO playerStatsSO = playerStats.StatsSO;
        _playerStats = playerStats;
        _playerStatsSO = playerStatsSO;
        _levelUpWindows = GetComponentsInChildren<LevelUpStatPanel>();
        foreach (var window in _levelUpWindows)
        {
            window.Initialize(playerStatsSO, playerStats, _levelUpWindowUI, _rarityConfigSO);
        }

        RerollPanels();
    }
    public void RerollPanels()
    {
        List<StatType> availableStats = _playerStatsSO.GetStatsByTag(StatTag.Primary);

        Shuffle(availableStats);

        int panelsToFill = Mathf.Min(_levelUpWindows.Length, availableStats.Count);

        for (int i = 0; i < panelsToFill; i++)
        {
            Rarity rarity = _rarityConfigSO.GetRandomEntry().Rarity;
            _levelUpWindows[i].RerollPanel(rarity, availableStats[i]);
        }
    }
    private void Shuffle(List<StatType> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
