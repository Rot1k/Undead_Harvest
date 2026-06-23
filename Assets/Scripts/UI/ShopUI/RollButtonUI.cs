using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

[RequireComponent(typeof(Button))]
public class RollButtonUI : MonoBehaviour
{
    [SerializeField] private BuyWindowsHolder _buyWindowsHolder;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private int _baseRerollCost = 15;

    private WalletManager _walletManager;

    private Button _rollButton;
    private int _rerollCost = 0;

    [Inject]
    public void Construct(WalletManager walletManager)
    {
        _walletManager = walletManager;
    }

    private void Awake()
    {
        _rollButton = GetComponent<Button>();
        _buttonText.text = $"Reroll ({_rerollCost})";
        _rollButton.onClick.AddListener(() =>
        {
            if (_walletManager.TrySpendMoney(_rerollCost))
            {
                _buyWindowsHolder.RerollWindows();
                _rerollCost = Mathf.RoundToInt(_rerollCost * 1.3f);
                _buttonText.text = $"Reroll ({_rerollCost})";
            }
            else
            {
                Debug.Log("Not enough money to reroll!");
            }

        });
    }
    private void OnEnable()
    {
        _rerollCost = _baseRerollCost;
        _buttonText.text = $"Reroll ({_rerollCost})";
    }
}
