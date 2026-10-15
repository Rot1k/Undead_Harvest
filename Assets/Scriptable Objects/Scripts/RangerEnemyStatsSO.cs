using UnityEngine;

[CreateAssetMenu(fileName = "RangerEnemyStatsSO", menuName = "Scriptable Objects/Enemy/RangerEnemyStatsSO")]
public class RangerEnemyStatsSO : EnemyStatsSO
{
    [Min(0.1f)] public float ShootingRange = 5f;
    [Min(0.1f)] public float RunAwayDistance = 3f;
    [Min(0.01f)] public float FireRate = 1.0f;
    [Min(0.1f)] public float BulletSpeed = 7f;
}
