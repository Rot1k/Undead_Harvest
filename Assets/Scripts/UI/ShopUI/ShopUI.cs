using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ShopUI : MonoBehaviour
{
    public event Action OnClosed;

    [SerializeField] private Button _closeButton;
    
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(EquipmentManager equipmentManager)
    {
        _equipmentManager = equipmentManager;
    }

    private void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
    }

    public void Dispose()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        for (int i = 0; i < _equipmentManager.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, _equipmentManager.GetWeapon(i));
        }
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
        OnClosed?.Invoke();
    }
}
