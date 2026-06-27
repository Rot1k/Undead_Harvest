using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ItemInfoUI : MonoBehaviour
{
    public Action OnClose;

    [SerializeField] private Vector2 _offset = new(-200f, 580f);
    [SerializeField] private Canvas _canvas;

    [SerializeField] private RarityConfigSO _rarityConfig;
    [SerializeField] private TextMeshProUGUI _itemNameText;
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;
    [SerializeField] private TextMeshProUGUI _itemTypeText;
    [SerializeField] private TextMeshProUGUI _itemRarityText;
    [SerializeField] private TextMeshProUGUI _itemSellCostText;
    [SerializeField] private Outline _backgroundOutline;
    [SerializeField] private Image _itemIconImage;
    [SerializeField] private Button _unionButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Button _closeButton;

    private InventorySlot _inventorySlot;
    private int _secondWeaponIndex = -1;

    private PlayerStatsSO _playerStatsSO;
    private EquipmentManager _equipmentManager;
    private IShopService _shopService;

    private int _sellCost;

    [Inject]
    public void Construct(EquipmentManager equipmentManager, IShopService shopService)
    {
        _equipmentManager = equipmentManager;
        _shopService = shopService;
    }

    private void Awake()
    {
        if (_unionButton != null)
            _unionButton.onClick.AddListener(Union);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(Close);

        if (_sellButton != null)
            _sellButton.onClick.AddListener(Sell);

        Hide();
    }

    public void Initialize(PlayerStatsSO playerStats)
    {
        _playerStatsSO = playerStats;
    }

    public void Close()
    {
        OnClose?.Invoke();
    }

    public void Show(InventorySlot slot, bool shopMode)
    {
        PositionNearSlot(slot);
        _secondWeaponIndex = -1;
        _inventorySlot = slot;
        if (_inventorySlot == null)
        {
            Debug.LogError("ItemInfoUI.Show called with null slot.");
            return;
        }

        InventoryItemSO slotData = ResolveSlotData(_inventorySlot);
        if (slotData == null)
        {
            Debug.LogWarning("No item found in the provided slot.");
            return;
        }

        if (_itemNameText != null) _itemNameText.text = slotData.ItemName;

        if (_itemRarityText != null)
        {
            _itemRarityText.text = slotData.Rarity.ToString();
            _itemRarityText.color = _rarityConfig != null ? _rarityConfig.GetColor(slotData.Rarity) : Color.white;
        }

        if (_backgroundOutline != null)
            _backgroundOutline.effectColor = _rarityConfig != null ? _rarityConfig.GetColor(slotData.Rarity) : Color.white;

        if (_itemIconImage != null) _itemIconImage.sprite = slotData.UISprite;

        if (_itemDescriptionText != null) _itemDescriptionText.text = ItemDescriptionBuilder.Build(slotData, _playerStatsSO);

        if (_itemTypeText != null)
        {
            string typeLabel = slotData is WeaponSO ? "Weapon" : "Item";
            _itemTypeText.text = typeLabel;
        }

        if (!shopMode)
        {
            gameObject.SetActive(true);
            if (_sellButton != null) _sellButton.gameObject.SetActive(false);
            if (_unionButton != null) _unionButton.gameObject.SetActive(false);
            return;
        }

        _sellCost = slotData.Price / 2;
        if (_itemSellCostText != null) _itemSellCostText.text = $"Sell {_sellCost}";

        if (_inventorySlot is WeaponInventorySlot weaponSlot)
        {
            WeaponSO weapon = _equipmentManager.GetWeapon(weaponSlot.SlotIndex);
            _secondWeaponIndex = _equipmentManager.FindSecondWeaponIndex(weapon, weaponSlot.SlotIndex);

            bool canUnion = weapon != null && _secondWeaponIndex != -1 && weapon.CanUnion;

            if (_unionButton != null)
            {
                _unionButton.gameObject.SetActive(true);
                _unionButton.interactable = canUnion;
            }
        }
        else if (_unionButton != null)
        {
            _unionButton.gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
    }

    private InventoryItemSO ResolveSlotData(InventorySlot slot)
    {
        switch (slot)
        {
            case WeaponInventorySlot weaponSlot:
                return _equipmentManager.GetWeapon(weaponSlot.SlotIndex);
            case ItemInventorySlot itemSlot:
                return itemSlot.Item.PassiveItemSO;
            default:
                Debug.LogError("Unsupported inventory slot type.");
                return null;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Sell()
    {
        if (_inventorySlot == null)
        {
            Debug.LogWarning("Sell called without a selected slot.");
            return;
        }

        if (_shopService == null)
        {
            Debug.LogWarning("Shop service is not available.");
            return;
        }

        if (_shopService.TrySellItem(_inventorySlot, out int soldAmount, out string error))
        {
            _sellCost = soldAmount;
            Close();
            return;
        }

        Debug.LogWarning(error);
    }

    private void Union()
    {
        if (_inventorySlot == null)
        {
            Debug.LogWarning("Union called without a selected slot.");
            return;
        }

        if (_shopService == null)
        {
            Debug.LogWarning("Shop service is not available.");
            return;
        }

        if (_shopService.TryUnion(_inventorySlot, out string error))
        {
            Close();
            return;
        }

        Debug.LogWarning(error);
    }

    private void PositionNearSlot(InventorySlot slot)
    {
        RectTransform slotRect = (RectTransform)slot.transform;
        RectTransform windowRect = (RectTransform)transform;

        Vector3 worldPos = slotRect.TransformPoint(slotRect.rect.center);
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_canvas.transform,
            screenPoint,
            null,
            out Vector2 localPoint
        );

        windowRect.anchoredPosition = localPoint + _offset;
    }
}
