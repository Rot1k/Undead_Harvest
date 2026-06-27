using UnityEngine;

public interface IShopService
{
    void Initialize(InventoryItemsPoolSO itemsPool, RarityConfigSO rarityConfig, PlayerStatsSO playerStats);
    InventoryItemSO RollRandomItem();
    int CalculateItemPrice(InventoryItemSO item);
    bool TryBuyItem(InventoryItemSO item, out string error);
    bool TrySellItem(InventorySlot slot, out int soldAmount, out string error);
    bool TryUnion(InventorySlot slot, out string error);
}