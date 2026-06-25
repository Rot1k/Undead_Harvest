using NTC.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static WaveConfigSO;


public class WavesManager : MonoBehaviour
{

    public event Action OnWaveCompleted;
    public event Action OnWaveStarted;
    public event Action OnAllWavesCompleted;

    public bool IsAllWavesCompleted { get; private set; } = false;

    [SerializeField] private Transform _playerTransform;
    [SerializeField] private WaveConfigSO[] _waves;

    private Vector2 _mapMin = new Vector2(-10f, -6f);
    private Vector2 _mapMax = new Vector2(10f, 6f);
    private float _spawnRadiusFromPlayer = 1.5f;
    public int CurrentWave { get; private set; } = 0;
    private List<Coroutine> _spawnCoroutines = new List<Coroutine>();
    private Coroutine _waveCoroutine;
    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
    }

    private void Start()
    {
        StartCoroutine(StartFirstWave());
    }

    private IEnumerator StartFirstWave()
    {
        if(_waveCoroutine != null)
        {
            Debug.LogWarning("A wave is already running on Start!");
            yield break;
        }

        yield return null;
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (_waves == null || _waves.Length == 0)
        {
            Debug.LogWarning("No waves configured!");
            return;
        }
        if (_waveCoroutine != null)
        {
            Debug.LogWarning("A wave is already running!");
            return;
        }
        _waveCoroutine = StartCoroutine(RunWave());
    }
    private IEnumerator RunWave()
    {
        OnWaveStarted?.Invoke();
        var wave = _waves[CurrentWave];
        Debug.Log($"Starting wave {CurrentWave + 1}/{_waves.Length}");

        foreach (var entry in wave.Enemies)
        {

            var routine = StartCoroutine(SpawnEnemyRoutine(entry, wave.WaveDuration));
            _spawnCoroutines.Add(routine);
            yield return new WaitForSeconds(0.5f);
        }

        if(wave.WaveDuration <= 0)
            yield break;

        yield return new WaitForSeconds(wave.WaveDuration);
        EndCurrentWave();
    }

    private IEnumerator SpawnEnemyRoutine(WaveEntry entry, float waveDuration)
    {
        if (entry == null || entry.EnemyStats == null || entry.EnemyStats.EnemyPrefab == null)
            yield break;

        float elapsedTime = 0f;
        

        yield return new WaitForSeconds(1f);

        while (waveDuration <= 0f || elapsedTime < waveDuration)
        {
            for (int i = 0; i < entry.EnemiesPerSpawn; i++)
            {
                Vector2 spawnPosition = Vector2.zero;
                bool foundPosition = false;

                for (int attempt = 0; attempt < 10; attempt++)
                {
                    spawnPosition = new Vector2(
                        UnityEngine.Random.Range(_mapMin.x, _mapMax.x),
                        UnityEngine.Random.Range(_mapMin.y, _mapMax.y)
                    );

                    if (Vector2.Distance(_playerTransform.position, spawnPosition) >= _spawnRadiusFromPlayer)
                    {
                        foundPosition = true;
                        break;
                    }
                }


                if (EnemyRegistry.CountByType(entry.EnemyStats) >= entry.maxEnemiesInScene)
                    continue;

                if (foundPosition)
                {
                    var enemyGO = NightPool.Spawn(
                        entry.EnemyStats.EnemyPrefab,
                        spawnPosition,
                        Quaternion.identity
                    );
                    _objectResolver?.InjectGameObject(enemyGO);


                    if (enemyGO.TryGetComponent<Enemy>(out var enemy))
                    {
                        enemy.Init(_playerTransform);
                        if (enemy is BossEnemy)
                        {
                            enemy.OnDied += OnBossDied;

                            void OnBossDied(Enemy boss)
                            {
                                boss.OnDied -= OnBossDied;
                                EndCurrentWave();
                            }
                        }
                        else
                        {
                            enemy.OnDied += OnEnemyDied;

                            void OnEnemyDied(Enemy e)
                            {
                                e.OnDied -= OnEnemyDied;
                            }
                        }
                    }
                }
            }
            if (entry.SpawnInterval <= 0f)
                yield break;
            yield return new WaitForSeconds(entry.SpawnInterval);
            elapsedTime += entry.SpawnInterval;
        }
    }
    private void DespawnAllEntities()
    {
        NightPool.ForEachPool(pool =>
        {
            // Skip the pool that uses Weapon prefab
            if (pool.AttachedPrefab.TryGetComponent<Weapon>(out _))
                return;
            if(pool.AttachedPrefab.TryGetComponent<ItemInventorySlot>(out _))
                return;

            try
            {
                pool.DespawnAllClones();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error despawning pool '{pool.AttachedPrefab.name}': {ex.Message}");
            }
        });
    }
    private void EndCurrentWave()
    {
        if (_waveCoroutine == null)
        {
            return;
        }

        foreach (var spawnCoroutine in _spawnCoroutines)
        {
            StopCoroutine(spawnCoroutine);
        }

        _spawnCoroutines.Clear();
        OnWaveCompleted?.Invoke();
        CurrentWave++;
        if (CurrentWave >= _waves.Length)
        {
            Debug.Log("All waves completed!");
            IsAllWavesCompleted = true;
            OnAllWavesCompleted?.Invoke();
        }
        _waveCoroutine = null;
        DespawnAllEntities();
    }
    public WaveConfigSO GetCurrentWave()
    {
        if (CurrentWave < 0 || CurrentWave >= _waves.Length)
            return null;
        return _waves[CurrentWave];
    }
}
