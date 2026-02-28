using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] protected ItemInfoController _itemInfoController;
    public RectTransform RectTransform => (RectTransform)transform;
    public Image ItemImage;
    public Outline SlotOutline;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_itemInfoController == null)
        {
            Debug.LogError("InventorySlot: ItemInfoUI reference not set.");
            return;
        }

        if (eventData.button == PointerEventData.InputButton.Right ||
            eventData.button == PointerEventData.InputButton.Left)
        {
            _itemInfoController.Toggle(this);
        }
    }
}
