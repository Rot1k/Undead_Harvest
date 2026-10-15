using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "WaveConfigSO", menuName = "Scriptable Objects/WaveConfigSO")]
public class WaveConfigSO : ScriptableObject
{
    [Serializable]
    public class WaveEntry
    {
        public EnemyStatsSO EnemyStats;
        [Min(-1f)] public float SpawnInterval = 2f; //if -1, spawn 1 time
        [Min(1)]
        public int EnemiesPerSpawn = 5;
        [FormerlySerializedAs("maxEnemiesInScene")]
        [Min(1)]
        public int MaxEnemiesInScene = 30;
        public int maxEnemiesInScene
        {
            get => MaxEnemiesInScene;
            set => MaxEnemiesInScene = value;
        }
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
        return Enemies.Find(e => e.EnemyStats == enemy)?.MaxEnemiesInScene ?? 20;
    }

    [Min(-1)] public int WaveDuration = 20;// if -1, infinite
    public bool IsBossWave = false;

    private void OnValidate()
    {
        if (Enemies == null)
            Enemies = new List<WaveEntry>();

        Enemies.RemoveAll(entry => entry == null);

        foreach (var entry in Enemies)
        {
            if (entry.EnemyStats == null)
                Debug.LogError($"{name} contains a wave entry without EnemyStats.", this);

            if (entry.SpawnInterval < 0f)
                entry.SpawnInterval = -1f;
        }
    }
}
