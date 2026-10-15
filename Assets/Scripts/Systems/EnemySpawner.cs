using NTC.Pool;
using System;
using System.Collections;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using static WaveConfigSO;

public sealed class EnemySpawner
{
    private readonly IObjectResolver _objectResolver;

    public EnemySpawner(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
    }

    public IEnumerator SpawnRoutine(
        WaveEntry entry,
        float waveDuration,
        Transform playerTransform,
        Vector2 mapMin,
        Vector2 mapMax,
        float spawnRadiusFromPlayer,
        Action onBossDied)
    {
        if (entry == null ||
            entry.EnemyStats == null ||
            entry.EnemyStats.EnemyPrefab == null ||
            playerTransform == null)
        {
            yield break;
        }

        float elapsedTime = 0f;
        yield return new WaitForSeconds(2f);

        while (waveDuration <= 0f || elapsedTime < waveDuration)
        {
            for (int i = 0; i < entry.EnemiesPerSpawn; i++)
            {
                Vector2 spawnPosition = Vector2.zero;
                bool foundPosition = false;

                for (int attempt = 0; attempt < 10; attempt++)
                {
                    spawnPosition = new Vector2(
                        UnityEngine.Random.Range(mapMin.x, mapMax.x),
                        UnityEngine.Random.Range(mapMin.y, mapMax.y)
                    );

                    if (Vector2.Distance(playerTransform.position, spawnPosition) >= spawnRadiusFromPlayer)
                    {
                        foundPosition = true;
                        break;
                    }
                }

                if (EnemyRegistry.CountByType(entry.EnemyStats) >= entry.maxEnemiesInScene)
                    continue;

                if (!foundPosition)
                    continue;

                var enemyGO = NightPool.Spawn(
                    entry.EnemyStats.EnemyPrefab,
                    spawnPosition,
                    Quaternion.identity
                );
                _objectResolver?.InjectGameObject(enemyGO);

                if (!enemyGO.TryGetComponent<Enemy>(out var enemy))
                    continue;

                enemy.Init(playerTransform);
                if (enemy is BossEnemy)
                {
                    Action<Enemy> onBossDiedHandler = null;
                    onBossDiedHandler = boss =>
                    {
                        boss.OnDied -= onBossDiedHandler;
                        onBossDied?.Invoke();
                    };
                    enemy.OnDied += onBossDiedHandler;
                }
            }

            if (entry.SpawnInterval <= 0f)
                yield break;

            yield return new WaitForSeconds(entry.SpawnInterval);
            elapsedTime += entry.SpawnInterval;
        }
    }
}
