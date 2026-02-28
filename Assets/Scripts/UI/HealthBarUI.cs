using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private PlayerHealthSystem _playerHealthSystem;

    private HealthSystem _healthSystem;

    private void Awake()
    {
        _healthSystem = _playerHealthSystem.HealthSystem;
    }
    private void OnEnable()
    {
        _healthSystem.OnHealthChanged += UpdateHealthUI;
    }
    private void OnDisable()
    {
        _healthSystem.OnHealthChanged -= UpdateHealthUI;
    }
    private void Start()
    {
        UpdateHealthUI(this, EventArgs.Empty);
    }
    private void UpdateHealthUI(object sender, EventArgs e)
    {
        _healthFillImage.fillAmount = _healthSystem.GetHealthPercent();
        _healthText.text = $"{_healthSystem.Health} / {_healthSystem.HealthMax}";
    }

}
