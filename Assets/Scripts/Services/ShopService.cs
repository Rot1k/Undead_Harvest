using System.Linq;
using UnityEngine;

public class ShopService : IShopService
{

    private const float WEAPON_CHANCE = 0.35f;

    private readonly WavesManager _wavesManager;
    private readonly EquipmentManager _equipmentManager;
    private readonly WalletManager _walletManager;
    private readonly SoundManager _soundManager;

    private InventoryItemsPoolSO _itemsPool;
    private RarityConfigSO _rarityConfig;

    public ShopService(
        WavesManager wavesManager,
        EquipmentManager equipmentManager,
        WalletManager walletManager,
        SoundManager soundManager)
    {
        _wavesManager = wavesManager;
        _equipmentManager = equipmentManager;
        _walletManager = walletManager;
        _soundManager = soundManager;
    }

    public void Initialize(InventoryItemsPoolSO itemsPool, RarityConfigSO rarityConfig, PlayerStatsSO playerStats)
    {
        _itemsPool = itemsPool;
        _rarityConfig = rarityConfig;
        _ = playerStats;
    }

    public InventoryItemSO RollRandomItem()
    {
        if (_itemsPool == null || _itemsPool.Items == null || _itemsPool.Items.Length == 0)
        {
            Debug.LogWarning("Items pool is empty or not assigned.");
            return null;
        }

        bool wantWeapon = Random.value < WEAPON_CHANCE;
        Rarity rolledRarity = _rarityConfig != null ? _rarityConfig.GetRandomEntry().Rarity : Rarity.Common;

        var candidates = _itemsPool.Items
            .Where(i => wantWeapon ? i is WeaponSO : i is PassiveItemSO)
            .ToList();

        if (candidates.Count == 0)
            candidates = _itemsPool.Items.ToList();

        var rarityFiltered = candidates.Where(i => i.Rarity == rolledRarity).ToList();
        if (rarityFiltered.Count > 0)
            candidates = rarityFiltered;

        return candidates[Random.Range(0, candidates.Count)];
    }

    public int CalculateItemPrice(InventoryItemSO item)
    {
        if (item == null)
            return 0;

        float basePrice = item.Price;
        int currentWave = _wavesManager != null ? _wavesManager.CurrentWave : 0;
        return Mathf.RoundToInt(basePrice + currentWave + (basePrice * 0.1f * currentWave));
    }

    public bool TryBuyItem(InventoryItemSO item, out string error)
    {
        error = null;
        if (item == null)
        {
            error = "No item selected";
            return false;
        }

        if (_walletManager == null)
        {
            error = "Wallet is not available";
            return false;
        }

        int price = CalculateItemPrice(item);
        if (_walletManager.Balance < price)
        {
            error = "Not enough money";
            return false;
        }

        switch (item)
        {
            case PassiveItemSO passive:
                _equipmentManager.AddItem(passive);
                break;
            case WeaponSO weapon:
                int emptySlot = _equipmentManager.GetFirstEmptyWeaponSlot();
                if (emptySlot != -1)
                {
                    _equipmentManager.EquipWeapon(weapon, emptySlot);
                    break;
                }

                int sameWeaponIndex = _equipmentManager.FindWeaponIndex(weapon);
                if (sameWeaponIndex == -1 || !weapon.CanUnion)
                {
                    error = "No empty slots and cannot union";
                    return false;
                }

                _equipmentManager.ReplaceWeapon(sameWeaponIndex, weapon.UnionResult);
                break;
            default:
                error = "Unsupported item type";
                return false;
        }

        _walletManager.TrySpendMoney(price);
        _soundManager?.PlaySound(SoundType.BUY);
        return true;
    }

    public bool TrySellItem(InventorySlot slot, out int soldAmount, out string error)
    {
        soldAmount = 0;
        error = null;

        if (slot == null)
        {
            error = "No slot";
            return false;
        }

        InventoryItemSO data = slot switch
        {
            WeaponInventorySlot w => _equipmentManager.GetWeapon(w.SlotIndex),
            ItemInventorySlot i => i.Item.PassiveItemSO,
            _ => null
        };

        if (data == null)
        {
            error = "No item to sell";
            return false;
        }

        soldAmount = data.Price / 2;

        switch (slot)
        {
            case WeaponInventorySlot ws:
                _equipmentManager.UnequipWeapon(ws.SlotIndex);
                break;
            case ItemInventorySlot islot:
                _equipmentManager.RemovePassiveItem(islot.Item);
                break;
            default:
                error = "Unsupported inventory slot type";
                return false;
        }

        _walletManager.AddMoney(soldAmount);
        return true;
    }

    public bool TryUnion(InventorySlot slot, out string error)
    {
        error = null;

        if (slot is not WeaponInventorySlot weaponSlot)
        {
            error = "Invalid slot for union";
            return false;
        }

        WeaponSO weapon = _equipmentManager.GetWeapon(weaponSlot.SlotIndex);
        if (weapon == null)
        {
            error = "No weapon in the selected slot";
            return false;
        }

        if (!weapon.CanUnion)
        {
            error = "This weapon cannot union";
            return false;
        }

        int secondWeaponIndex = _equipmentManager.FindSecondWeaponIndex(weapon, weaponSlot.SlotIndex);
        if (secondWeaponIndex == -1)
        {
            error = "No matching weapon to union";
            return false;
        }

        _equipmentManager.UnequipWeapon(secondWeaponIndex);
        _equipmentManager.ReplaceWeapon(weaponSlot.SlotIndex, weapon.UnionResult);
        return true;
    }
}
