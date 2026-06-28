using TMPro;
using UnityEngine;
using VContainer;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balanceText;

    private WalletManager _walletManager;

    [Inject]
    public void Construct(WalletManager walletManager)
    {
        _walletManager = walletManager;
    }

    public void Initialize()
    {
        if (_walletManager != null)
            _walletManager.OnBalanceChanged += UpdateUI;
        if (_walletManager != null)
            UpdateUI(_walletManager.Balance);
    }

    private void Start()
    {
        // Initialization handled in Initialize called by UIBootstrap
    }

    public void Dispose()
    {
        if (_walletManager != null)
            _walletManager.OnBalanceChanged -= UpdateUI;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void UpdateUI(float newBalance)
    {
        _balanceText.text = Mathf.Floor(newBalance).ToString();
    }
}
