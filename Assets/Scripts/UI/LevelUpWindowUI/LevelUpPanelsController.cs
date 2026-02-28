using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUpPanelsController : MonoBehaviour
{
    [SerializeField] private LevelUpWindowUI _levelUpWindowUI;
    [SerializeField] private PlayerStatsSO _playerStatsSO;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private RarityConfigSO _rarityConfigSO;


    private LevelUpStatPanel[] _levelUpWindows;

    private void Awake()
    {
        _levelUpWindows = GetComponentsInChildren<LevelUpStatPanel>();
        foreach (var window in _levelUpWindows)
        {
            window.Initialize(_playerStatsSO, _playerStats, _levelUpWindowUI, _rarityConfigSO);
        }
    }
    private void Start()
    {
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
