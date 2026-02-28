using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private PlayerLevelSystem _playerLevelSystem;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _expFillImage;

    private LevelSystem _levelSystem;
    private void Awake()
    {
        _levelSystem = _playerLevelSystem.LevelSystem;
    }
    private void OnEnable()
    {
        _levelSystem.OnExpChanged += UpdateExpUI;
        _levelSystem.OnLevelChanged += UpdateLevelUI;
    }
    private void OnDisable()
    {
        _levelSystem.OnExpChanged -= UpdateExpUI;
        _levelSystem.OnLevelChanged -= UpdateLevelUI;
    }
    private void Start()
    {
        UpdateLevelUI(this, System.EventArgs.Empty);
        UpdateExpUI(this, System.EventArgs.Empty);
    }
    private void UpdateLevelUI(object sender, System.EventArgs e)
    {
        _levelText.text = $"LV. {_levelSystem.Level}";
    }
    private void UpdateExpUI(object sender, System.EventArgs e)
    {
        _expFillImage.fillAmount = _levelSystem.GetExpPercent();
    }
}
