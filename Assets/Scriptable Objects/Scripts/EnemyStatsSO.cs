using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/Enemy/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    public GameObject EnemyPrefab;

    public int BaseHealth = 100;
    public int BaseDamage = 1;
    public float BaseMoveSpeed = 5f;
}
