using UnityEngine;

public class BuyWindowsHolder : MonoBehaviour
{
    [SerializeField] private BuyWindow[] _buyWindows;
    [SerializeField] private InventoryItemsPoolSO _itemsPool;
    [SerializeField] private RarityConfigSO _rarityConfigSO;

    private void Awake()
    {
        foreach (var window in _buyWindows)
        {
            window.Initialize(_itemsPool, _rarityConfigSO);
        }
    }
    private void OnEnable()
    {
        RerollWindows();
    }
    public void RerollWindows()
    {
        foreach (var window in _buyWindows)
        {
            if(window.IsLocked)
                continue;
            window.SetRandomItem();
        }
    }
}
