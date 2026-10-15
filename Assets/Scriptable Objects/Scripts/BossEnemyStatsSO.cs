using UnityEngine;

[CreateAssetMenu(fileName = "BossEnemyStatsSO", menuName = "Scriptable Objects/Enemy/BossEnemyStatsSO")]
public class BossEnemyStatsSO : EnemyStatsSO
{
    [Min(0.01f)] public float FireRate = 1.0f;
    [Min(0.1f)] public float BulletSpeed = 7f;
}
