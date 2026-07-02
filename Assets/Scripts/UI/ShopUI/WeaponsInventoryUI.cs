using UnityEngine;
using VContainer;

public class WeaponsInventoryUI : MonoBehaviour
{
    [SerializeField] private WeaponInventorySlot[] _slots;
    [SerializeField] private RarityConfigSO _rarityConfig;

    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(EquipmentManager equipmentManager)
    {
        _equipmentManager = equipmentManager;
    }

    public void Initialize()
    {

        _equipmentManager.OnWeaponEquipped += UpdateUI;
        _equipmentManager.OnWeaponUnequipped += UpdateUI;
    }


    public void Dispose()
    {
        if (_equipmentManager == null) return;

        _equipmentManager.OnWeaponEquipped -= UpdateUI;
        _equipmentManager.OnWeaponUnequipped -= UpdateUI;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void UpdateUI(int slot, WeaponSO weapon)
    {
        if (_slots == null || _slots.Length == 0)
        {
            Debug.LogError("[WeaponsInventoryUI] Slots array is not assigned.");
            return;
        }
        if (slot < 0 || slot >= _slots.Length)
        {
            Debug.LogError($"[WeaponsInventoryUI] Slot index out of range: {slot}. Slots length: {_slots.Length}");
            return;
        }
        if (_slots[slot] == null || _slots[slot].ItemImage == null)
        {
            Debug.LogError($"[WeaponsInventoryUI] Slot {slot} or ItemImage is not assigned.");
            return;
        }

        if (weapon == null)
        {
            _slots[slot].ItemImage.sprite = null;
            _slots[slot].ItemImage.enabled = false;
            _slots[slot].SlotOutline.effectColor = Color.clear;
        }
        else
        {
            _slots[slot].ItemImage.sprite = weapon.UISprite;
            _slots[slot].ItemImage.enabled = true;
            Color outlineColor = _rarityConfig.GetColor(weapon.Rarity);
            _slots[slot].SlotOutline.effectColor = outlineColor;
        }
    }
}
