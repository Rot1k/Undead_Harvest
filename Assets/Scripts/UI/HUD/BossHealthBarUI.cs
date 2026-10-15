using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BossHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image _healthFillImage;

    private WavesManager _wavesManager;
    private BossEnemy _boss;
    private HealthSystem _healthSystem;

    private CompositeDisposable _bossDisposables = new();

    [Inject]
    public void Construct(WavesManager wavesManager)
    {
        _wavesManager = wavesManager;
    }

    public void Initialize()
    {
        _wavesManager.OnWaveStarted += OnWaveStarted;
        BossEnemy.OnBossSpawned += BindToBoss;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _wavesManager.OnWaveStarted -= OnWaveStarted;
        BossEnemy.OnBossSpawned -= BindToBoss;

        _bossDisposables.Dispose();
    }

    private void OnWaveStarted()
    {
        gameObject.SetActive(false);
        Unbind();
    }

    private void BindToBoss(BossEnemy boss)
    {
        var wave = _wavesManager.GetCurrentWave();

        if (wave == null || !wave.IsBossWave)
            return;

        Unbind();

        _boss = boss;
        _healthSystem = boss.HealthSystem;

        Observable.CombineLatest(
                _healthSystem.Health,
                _healthSystem.HealthMax,
                (health, maxHealth) => (health, maxHealth))
            .Subscribe(UpdateHealthUI)
            .AddTo(_bossDisposables);

        _boss.OnDied += OnBossDied;

        gameObject.SetActive(true);
    }

    private void OnBossDied(Enemy enemy)
    {
        gameObject.SetActive(false);
        Unbind();
    }

    private void Unbind()
    {
        _bossDisposables.Dispose();
        _bossDisposables = new();

        if (_boss != null)
            _boss.OnDied -= OnBossDied;

        _boss = null;
        _healthSystem = null;
    }

    private void UpdateHealthUI((int health, int maxHealth) values)
    {
        _healthFillImage.fillAmount = (float)values.health / values.maxHealth;
    }
}