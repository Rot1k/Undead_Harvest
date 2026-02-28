using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpStatPanel : MonoBehaviour, IHaveRarity
{
    public Rarity Rarity { get; private set; }
    public StatType StatType { get; private set; }

    [SerializeField] private Image _icon;
    [SerializeField] private Button _pickButton;
    [SerializeField] private TextMeshProUGUI _statTypeText;
    [SerializeField] private TextMeshProUGUI _rarityText;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Outline _backgroundOutline;

    private LevelUpWindowUI _levelUpWindowUI;
    private PlayerStats _playerStats;
    private PlayerStatsSO _statsSO;
    private RarityConfigSO _rarityConfigSO;
    private Color _rarityColor;
    private float _baseStatIncreaseAmount;
    private float _statIncreaseAmountMultiplier = 1f;
    private float _totalStatIncreaseAmount => _baseStatIncreaseAmount * _statIncreaseAmountMultiplier;

    private void Awake()
    {
        _pickButton.onClick.AddListener(() =>
        {
            if (_playerStats != null)
            {
                _levelUpWindowUI.PlayerLevelSystem.LevelSystem.UseSkillPoint(1);
                _playerStats.SetBaseStat(StatType, _playerStats.Get(StatType) + _totalStatIncreaseAmount);
                Debug.Log($"Increased {StatType} by {_totalStatIncreaseAmount}");
                Debug.Log($"New {StatType} value: {_playerStats.Get(StatType)}");
                _levelUpWindowUI.TryRerollPanels();
            }
        });
    }

    public void Initialize(PlayerStatsSO playerStatsSO, PlayerStats playerStats, LevelUpWindowUI levelUpWindowUI, RarityConfigSO rarityConfigSO)
    {
        _statsSO = playerStatsSO;
        _playerStats = playerStats;
        _levelUpWindowUI = levelUpWindowUI;
        _rarityConfigSO = rarityConfigSO;
    }
    public void RerollPanel(Rarity rarity, StatType statType)
    {
        Rarity = rarity;
        StatType = statType;

        if (_statsSO != null && _statsSO.Stats.Find(s => s.Type == StatType) is var entry && entry != null)
        {
            if (_icon != null)
                _icon.sprite = entry.Icon;
        }

        _statIncreaseAmountMultiplier = _rarityConfigSO.GetMultiplier(Rarity);
        _baseStatIncreaseAmount = _statsSO.GetBaseIncreaseAmount(StatType);

        _rarityColor = _rarityConfigSO.GetColor(Rarity);
        SetText();
    }
    private void SetText()
    {
        _rarityText.text = Rarity.ToString();
        _rarityText.color = _rarityColor;
        _statTypeText.text = _statsSO.GetName(StatType);
        _amountText.text = $"+{_totalStatIncreaseAmount:0.##}";
        _backgroundOutline.effectColor = _rarityColor;
    }
}
