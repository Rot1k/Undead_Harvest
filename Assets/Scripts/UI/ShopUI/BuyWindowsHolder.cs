using UnityEngine;

public class BuyWindowsHolder : MonoBehaviour
{
    [SerializeField] private BuyWindow[] _buyWindows;
    [SerializeField] private InventoryItemsPoolSO _itemsPool;
    [SerializeField] private RarityConfigSO _rarityConfigSO;
    [SerializeField] private PlayerStatsSO _playerStatsSO;

    private bool _isInitialized = false;

    private void Initialize()
    {
        if (_isInitialized)
            return;
        foreach (var window in _buyWindows)
        {
            window.Initialize(_itemsPool, _rarityConfigSO, _playerStatsSO);
        }
        _isInitialized = true;
    }

    private void OnEnable()
    {
        Initialize();
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
