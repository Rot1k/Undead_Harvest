using UnityEngine;

public class ItemInfoController : MonoBehaviour
{
    [SerializeField] private ItemInfoUI _itemInfoUI;
    [SerializeField] private PlayerStatsSO _playerStatsSO;
    [SerializeField] private bool _shopMode;

    private InventorySlot _currentSlot;
    
    private void Awake()
    {
        _itemInfoUI.OnClose += Hide;
        _itemInfoUI.Initialize(_playerStatsSO);
    }
    public void Toggle(InventorySlot slot)
    {
        if (_currentSlot == slot)
        {
            Hide();
            return;
        }

        _currentSlot = slot;
        _itemInfoUI.Show(slot, _shopMode);
    }

    public void Hide()
    {
        _currentSlot = null;
        _itemInfoUI.Hide();
    }
}
