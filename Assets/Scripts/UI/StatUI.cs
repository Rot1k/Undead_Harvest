using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatUI : MonoBehaviour
{
    [SerializeField] private StatType _type;
    [SerializeField] private TextMeshProUGUI _valueText;

    [SerializeField] private Image _icon;
    private PlayerStats _playerStats;
    private PlayerStatsSO _statsSO;
    private IDisposable _statSubscription;

    public void Initialize(PlayerStats playerStats, PlayerStatsSO statsSO)
    {
        _statSubscription?.Dispose();
        _playerStats = playerStats;
        _statsSO = statsSO;

        _icon.sprite = _statsSO.GetIcon(_type);
        if (_playerStats != null)
            _statSubscription = _playerStats.Observe(_type).Subscribe(value => OnStatChanged(value));
    }

    private void OnDestroy()
    {
        _statSubscription?.Dispose();
    }

    private void OnStatChanged(float newValue)
    {
        _valueText.text = newValue.ToString(_type == StatType.MaxHealth ? "0" : "0.##");
    }
}