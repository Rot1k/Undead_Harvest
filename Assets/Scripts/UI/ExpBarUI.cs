using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _expFillImage;

    private PlayerLevelSystem _playerLevelSystem;
    private LevelSystem _levelSystem;
    public void Initialize(PlayerLevelSystem playerLevelSystem)
    {
        _playerLevelSystem = playerLevelSystem;
        _levelSystem = _playerLevelSystem.LevelSystem;

        _levelSystem.OnExpChanged += UpdateExpUI;
        _levelSystem.OnLevelChanged += UpdateLevelUI;

        UpdateLevelUI(this, System.EventArgs.Empty);
        UpdateExpUI(this, System.EventArgs.Empty);
    }
    public void Dispose()
    {
        if (_levelSystem != null)
        {
            _levelSystem.OnExpChanged -= UpdateExpUI;
            _levelSystem.OnLevelChanged -= UpdateLevelUI;
        }
    }
    private void OnDestroy()
    {
        Dispose();
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
