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
    private List<Coroutine> _spawnCoroutines = new();
    private Coroutine _waveCoroutine;
    private EnemySpawner _enemySpawner;

    [Inject]
    public void Construct(EnemySpawner enemySpawner)
    {
        _enemySpawner = enemySpawner;
    }

    public void StartGame()
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
        if (_enemySpawner == null)
        {
            Debug.LogError("Enemy spawner is not configured.", this);
            return;
        }
        if (CurrentWave >= _waves.Length)
        {
            Debug.LogWarning("All waves have already been completed!");
            return;
        }
        _waveCoroutine = StartCoroutine(RunWave());
    }
    private IEnumerator RunWave()
    {
        OnWaveStarted?.Invoke();
        var wave = _waves[CurrentWave];
        if (wave == null)
        {
            Debug.LogError($"Wave {CurrentWave} is not configured.", this);
            EndCurrentWave();
            yield break;
        }

        Debug.Log($"Starting wave {CurrentWave + 1}/{_waves.Length}");
        if (wave.Enemies == null)
        {
            Debug.LogError($"Wave {CurrentWave} has no enemy entries.", this);
            EndCurrentWave();
            yield break;
        }

        foreach (var entry in wave.Enemies)
        {
            var routine = StartCoroutine(_enemySpawner.SpawnRoutine(
                entry,
                wave.WaveDuration,
                _playerTransform,
                _mapMin,
                _mapMax,
                _spawnRadiusFromPlayer,
                EndCurrentWave));
            _spawnCoroutines.Add(routine);
            yield return new WaitForSeconds(0.5f);
        }

        if (wave.WaveDuration <= 0)
        {
            EndCurrentWave();
            yield break;
        }

        float remainingDuration = Mathf.Max(0f, wave.WaveDuration - (0.5f * (wave.Enemies.Count - 1)));
        yield return new WaitForSeconds(remainingDuration);
        EndCurrentWave();
    }
    private void DespawnAllEntities()
    {
        NightPool.ForEachPool(pool =>
        {
            if (pool == null || pool.AttachedPrefab == null)
                return;

            // Skip the pool that uses Weapon prefab
            if (pool.AttachedPrefab.TryGetComponent<Weapon>(out _))
                return;
            if (pool.AttachedPrefab.TryGetComponent<ItemInventorySlot>(out _))
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
    public void StopCurrentWave()
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
        _waveCoroutine = null;
    }

    private void EndCurrentWave()
    {
        if (_waveCoroutine == null)
        {
            return;
        }

        StopCurrentWave();
        DespawnAllEntities();
        CurrentWave++;

        if (CurrentWave >= _waves.Length)
        {
            Debug.Log("All waves completed!");
            IsAllWavesCompleted = true;
            OnAllWavesCompleted?.Invoke();
        }
        else
        {
            OnWaveCompleted?.Invoke();
        }
    }
    public WaveConfigSO GetCurrentWave()
    {
        if (CurrentWave < 0 || CurrentWave >= _waves.Length)
            return null;
        return _waves[CurrentWave];
    }
}
