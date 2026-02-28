using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfigSO", menuName = "Scriptable Objects/WaveConfigSO")]
public class WaveConfigSO : ScriptableObject
{
    [Serializable]
    public class WaveEntry
    {
        public EnemyStatsSO EnemyStats;
        public float SpawnInterval = 2f; //if -1, spawn 1 time
        public int EnemiesPerSpawn = 5;
        public int maxEnemiesInScene = 30;
    }
    public List<WaveEntry> Enemies = new();

    public float GetSpawnInterval(EnemyStatsSO enemy)
    {
        return Enemies.Find(e => e.EnemyStats == enemy)?.SpawnInterval ?? 2f;
    }
    public int GetEnemiesPerSpawn(EnemyStatsSO enemy)
    {
        return Enemies.Find(e => e.EnemyStats == enemy)?.EnemiesPerSpawn ?? 5;
    }
    public int GetMaxEnemiesInScene(EnemyStatsSO enemy)
    {
        return Enemies.Find(e => e.EnemyStats == enemy)?.maxEnemiesInScene ?? 20;
    }

    public int WaveDuration = 20;// if -1, infinite
    public bool IsBossWave = false;
}
