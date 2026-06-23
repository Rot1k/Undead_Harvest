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
    private void Start()
    {
        _walletManager.OnBalanceChanged += UpdateUI;
        UpdateUI(_walletManager.Balance);
    }

    private void UpdateUI(float newBalance)
    {
        _balanceText.text = Mathf.Floor(newBalance).ToString();
    }
}
