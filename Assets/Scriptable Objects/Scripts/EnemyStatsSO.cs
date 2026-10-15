using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/Enemy/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public GameObject EnemyPrefab;

    [Min(1)] public int BaseHealth = 100;
    [Min(1)] public int BaseDamage = 1;
    [Min(0.01f)] public float BaseMoveSpeed = 5f;
}
