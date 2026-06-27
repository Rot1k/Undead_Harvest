using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BuyWindow : MonoBehaviour
{
    [SerializeField] private Button _buyButton;
    [SerializeField] private Toggle _lockButton;
    [SerializeField] private TextMeshProUGUI _lockButtonText;
    [SerializeField] private Image _itemImage;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Outline _backgroundOutline;
    [SerializeField] private TextMeshProUGUI _rarityText;

    private InventoryItemSO _currentItem;
    private RarityConfigSO _rarityConfig;
    private PlayerStatsSO _playerStatsSO;
    private IShopService _shopService;

    public bool IsLocked { get; private set; } = false;

    [Inject]
    public void Construct(IShopService shopService)
    {
        _shopService = shopService;
    }

    private void Awake()
    {
        _buyButton.onClick.AddListener(OnBuyButtonClicked);
        _lockButton.onValueChanged.AddListener(isOn =>
        {
            IsLocked = isOn;
            _lockButtonText.text = IsLocked ? "Unlock" : "Lock";
        });
    }

    private void OnBuyButtonClicked()
    {
        if (_currentItem == null)
        {
            Debug.LogWarning("No item selected to buy.");
            return;
        }

        string error = null;
        if (_shopService != null && _shopService.TryBuyItem(_currentItem, out error))
        {
            SetRandomItem();
            _lockButton.isOn = false;
            return;
        }

        Debug.LogWarning(error ?? "Shop service is not available.");
        _lockButton.isOn = false;
    }

    public void Initialize(InventoryItemsPoolSO itemsPool, RarityConfigSO rarityConfigSO, PlayerStatsSO playerStatsSO)
    {
        _rarityConfig = rarityConfigSO;
        _playerStatsSO = playerStatsSO;

        _shopService?.Initialize(itemsPool, rarityConfigSO, playerStatsSO);
    }

    public void SetItem(InventoryItemSO item)
    {
        if (item == null)
            return;

        if (_rarityConfig != null)
        {
            Color color = _rarityConfig.GetColor(item.Rarity);
            _backgroundOutline.effectColor = color;
            _rarityText.color = color;
        }

        _currentItem = item;
        _itemImage.sprite = item.UISprite;
        _itemNameText.text = item.ItemName;
        _itemTypeText.text = item is WeaponSO ? "Weapon" : "Item";
        _rarityText.text = item.Rarity.ToString();
        _descriptionText.text = ItemDescriptionBuilder.Build(item, _playerStatsSO);

        if (_shopService != null)
            _buyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy ({_shopService.CalculateItemPrice(item)})";
    }

    public void SetRandomItem()
    {
        if (_shopService == null)
            return;

        InventoryItemSO picked = _shopService.RollRandomItem();
        if (picked != null)
            SetItem(picked);
    }
}
