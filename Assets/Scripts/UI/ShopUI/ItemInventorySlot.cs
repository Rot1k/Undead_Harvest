using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemInventorySlot : InventorySlot
{   
    private readonly Stack<PassiveItemInstance> _instances = new();
    [SerializeField] private TextMeshProUGUI _itemNumberText;

    public PassiveItemInstance Item => _instances.Count > 0 ? _instances.Peek() : null;

    public void SetItem(PassiveItemInstance item, ItemInfoController itemInfoController)
    {
        _itemInfoController = itemInfoController;
        _instances.Clear();
        _instances.Push(item);
        ItemImage.sprite = item.PassiveItemSO.UISprite;
        UpdateCountDisplay();
    }

    public void PushInstance(PassiveItemInstance item)
    {
        _instances.Push(item);
        UpdateCountDisplay();
    }

    public PassiveItemInstance PopInstance()
    {
        var instance = _instances.Count > 0 ? _instances.Pop() : null;
        UpdateCountDisplay();
        return instance;
    }

    public int GetCount() => _instances.Count;

    private void UpdateCountDisplay()
    {
        if (_itemNumberText != null)
        {
            if (_instances.Count > 1)
            {
                _itemNumberText.text = _instances.Count.ToString();
                _itemNumberText.gameObject.SetActive(true);
            }
            else
            {
                _itemNumberText.gameObject.SetActive(false);
            }
        }
    }
}
