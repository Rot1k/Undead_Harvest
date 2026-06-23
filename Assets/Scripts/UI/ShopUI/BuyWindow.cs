using System.Linq;
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


    [SerializeField, Range(0f, 1f)] private float _weaponChance = 0.35f; 

    private InventoryItemSO _currentItem;
    private InventoryItemsPoolSO _itemsPool;
    private RarityConfigSO _rarityConfig;
    private PlayerStatsSO _playerStatsSO;
    private WavesManager _wavesManager;
    private EquipmentManager _equipmentManager;
    private WalletManager _walletManager;
    private SoundManager _soundManager;

    public bool IsLocked { get; private set; } = false;

    [Inject]
    public void Construct(WavesManager wavesManager, EquipmentManager equipmentManager, WalletManager walletManager, SoundManager soundManager)
    {
        _wavesManager = wavesManager;
        _equipmentManager = equipmentManager;
        _walletManager = walletManager;
        _soundManager = soundManager;
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
        int itemPrice = CalculateItemPrice(_currentItem);
        if (_walletManager.Balance >= itemPrice)
        {
            switch (_currentItem)
            {
                case PassiveItemSO itemSO:
                    _equipmentManager.AddItem(itemSO);
                    _walletManager.TrySpendMoney(itemPrice);
                    _soundManager.PlaySound(SoundType.BUY);
                    SetRandomItem();
                    break;
                case WeaponSO weaponSO:

                    int weaponSlot = _equipmentManager.GetFirstEmptyWeaponSlot();

                    if (weaponSlot == -1)
                    {
                        int secondWeaponIndex = _equipmentManager.FindWeaponIndex(weaponSO);
                        bool canUnion = (secondWeaponIndex != -1) && weaponSO.CanUnion;
                        if (!canUnion)
                        {
                            Debug.LogWarning("No empty weapon slots available!");
                            break;
                        }

                        _equipmentManager.ReplaceWeapon(secondWeaponIndex, weaponSO.UnionResult);
                    }
                    else
                    {
                        _equipmentManager.EquipWeapon(weaponSO, weaponSlot);
                    }

                    _walletManager.TrySpendMoney(itemPrice);
                    _soundManager.PlaySound(SoundType.BUY);
                    SetRandomItem();
                    break;
                default:
                    Debug.LogWarning("Unknown type of item!");
                    break;
            }
            _lockButton.isOn = false;
        }
    }

    public void Initialize(InventoryItemsPoolSO itemsPool, RarityConfigSO rarityConfigSO, PlayerStatsSO playerStatsSO)
    {
        _itemsPool = itemsPool;
        _rarityConfig = rarityConfigSO;
        _playerStatsSO = playerStatsSO;
    }

    public void SetItem(InventoryItemSO item)
    {
        Color color = _rarityConfig.GetColor(item.Rarity);
        _currentItem = item;
        _itemImage.sprite = item.UISprite;
        _itemNameText.text = item.ItemName;
        _buyButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Buy ({CalculateItemPrice(item)})";
        _backgroundOutline.effectColor = color;
        _rarityText.text = item.Rarity.ToString();
        _rarityText.color = color;
        string typeLabel = item is WeaponSO ? "Weapon" : "Item"; 
        _itemTypeText.text = typeLabel;
        _descriptionText.text = ItemDescriptionBuilder.Build(item, _playerStatsSO);
    }

    public void SetRandomItem()
    {
        if (_itemsPool == null || _itemsPool.Items == null || _itemsPool.Items.Length == 0)
        {
            Debug.LogWarning("Items pool is empty or not assigned.");
            return;
        }

        bool wantWeapon = Random.value < _weaponChance;

        Rarity rolledRarity = (_rarityConfig != null)
            ? _rarityConfig.GetRandomEntry().Rarity
            : Rarity.Common;

        var candidates = _itemsPool.Items
            .Where(i => wantWeapon ? i is WeaponSO : i is PassiveItemSO)
            .ToList();

        if (candidates.Count == 0)
            candidates = _itemsPool.Items.ToList();

        var rarityFiltered = candidates.Where(i => i.Rarity == rolledRarity).ToList();
        if (rarityFiltered.Count > 0)
            candidates = rarityFiltered;

        var picked = candidates[Random.Range(0, candidates.Count)];
        SetItem(picked);
    }
    private int CalculateItemPrice(InventoryItemSO item)
    {
        float basePrice = item.Price;
        int currentWave = _wavesManager.CurrentWave;
        return Mathf.RoundToInt(basePrice + currentWave + (basePrice * 0.1f * currentWave));
    }
}
