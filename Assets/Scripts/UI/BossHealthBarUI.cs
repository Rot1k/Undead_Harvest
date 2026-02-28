using System;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;

    private HealthSystem _healthSystem;
    private BossEnemy _boss;

    private void Awake()
    {
        WavesManager.Instance.OnWaveStarted += OnWaveStarted;
        BossEnemy.OnBossSpawned += BindToBoss;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (WavesManager.Instance != null)
            WavesManager.Instance.OnWaveStarted -= OnWaveStarted;

        BossEnemy.OnBossSpawned -= BindToBoss;
        Unbind();
    }

    private void OnWaveStarted()
    {
        gameObject.SetActive(false);
        Unbind();
    }

    private void BindToBoss(BossEnemy boss)
    {
        var wave = WavesManager.Instance.GetCurrentWave();
        if (wave == null || !wave.IsBossWave)
            return;

        Unbind();

        _boss = boss;
        _healthSystem = boss.HealthSystem;

        _healthSystem.OnHealthChanged += UpdateHealthUI;
        _boss.OnDied += OnBossDied;

        UpdateHealthUI(this, EventArgs.Empty);
        gameObject.SetActive(true);
    }

    private void OnBossDied(Enemy enemy)
    {
        Unbind();
        gameObject.SetActive(false);
    }

    private void Unbind()
    {
        if (_healthSystem != null)
            _healthSystem.OnHealthChanged -= UpdateHealthUI;

        if (_boss != null)
            _boss.OnDied -= OnBossDied;

        _healthSystem = null;
        _boss = null;
    }

    private void UpdateHealthUI(object sender, EventArgs e)
    {
        if (_healthSystem != null)
            _healthFillImage.fillAmount = _healthSystem.GetHealthPercent();
    }
}