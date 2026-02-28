using TMPro;
using UnityEngine;

public class WalletUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _balanceText;
    private void Start()
    {
        WalletManager.Instance.OnBalanceChanged += UpdateUI;
        UpdateUI(WalletManager.Instance.Balance);
    }

    private void UpdateUI(float newBalance)
    {
        _balanceText.text = Mathf.Floor(newBalance).ToString();
    }
}
