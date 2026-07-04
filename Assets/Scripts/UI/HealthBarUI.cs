using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;
    [SerializeField] private TextMeshProUGUI _healthText;

    private PlayerHealthSystem _playerHealthSystem;

    [Inject]
    public void Construct(PlayerHealthSystem playerHealthSystem)
    {
        _playerHealthSystem = playerHealthSystem;
    }

    public void Initialize()
    {
        var health = _playerHealthSystem.HealthSystem.Health;
        var max = _playerHealthSystem.HealthSystem.HealthMax;

        Observable.CombineLatest(health, max, (h, m) => (h, m))
            .Subscribe(UpdateHealthUI)
            .AddTo(this);

        UpdateHealthUI((health.CurrentValue, max.CurrentValue));
    }
    public void Dispose()
    {

    }
    private void OnDestroy()
    {
        Dispose();
    }

    private void UpdateHealthUI((int health, int max) values)
    {
        _healthFillImage.fillAmount = (float)values.health / values.max;
        _healthText.text = $"{values.health} / {values.max}";
    }

}
