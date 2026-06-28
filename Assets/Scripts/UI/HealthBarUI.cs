using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;
    [SerializeField] private TextMeshProUGUI _healthText;

    private PlayerHealthSystem _playerHealthSystem;
    private HealthSystem _healthSystem;

    public void Initialize(PlayerHealthSystem playerHealthSystem)
    {
        _playerHealthSystem = playerHealthSystem;
        _healthSystem = _playerHealthSystem.HealthSystem;

        _healthSystem.OnHealthChanged += UpdateHealthUI;
        UpdateHealthUI(this, EventArgs.Empty);
    }
    public void Dispose()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= UpdateHealthUI;

        }
    }
    private void OnDestroy()
    {
        Dispose();
    }

    private void UpdateHealthUI(object sender, EventArgs e)
    {
        _healthFillImage.fillAmount = _healthSystem.GetHealthPercent();
        _healthText.text = $"{_healthSystem.Health} / {_healthSystem.HealthMax}";
    }

}
