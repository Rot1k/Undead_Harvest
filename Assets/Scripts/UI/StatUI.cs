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
    private bool _subscribed;

    public void Initialize(PlayerStats playerStats, PlayerStatsSO statsSO)
    {
        Unsubscribe();
        _playerStats = playerStats;
        _statsSO = statsSO;

        _icon.sprite = _statsSO.GetIcon(_type);
        if (_playerStats != null)
            _valueText.text = _playerStats.Get(_type).ToString("0.##");

        Subscribe();
    }
    private void OnEnable()
    {
        Subscribe();
    }
    private void OnDisable()
    {
        Unsubscribe();
    }
    private void Subscribe()
    {
        if (_playerStats != null && !_subscribed)
        {
            _playerStats.OnStatChanged += OnStatChanged;
            _subscribed = true;
        }
    }
    private void Unsubscribe()
    {
        if (_playerStats != null && _subscribed)
        {
            _playerStats.OnStatChanged -= OnStatChanged;
            _subscribed = false;
        }
    }
    private void OnStatChanged(StatType type, float newValue)
    {
        if (type != _type) return;
        if (type == StatType.MaxHealth)
        {
            _valueText.text = newValue.ToString("0");
        }

        _valueText.text = newValue.ToString("0.##");
    }
}