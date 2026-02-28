using UnityEngine;

[CreateAssetMenu(fileName = "RangerEnemyStatsSO", menuName = "Scriptable Objects/Enemy/RangerEnemyStatsSO")]
public class RangerEnemyStatsSO : EnemyStatsSO
{
    public float ShootingRange = 5f;
    public float RunAwayDistance = 3f;
    public float FireRate = 1.0f;
    public float BulletSpeed = 7f;
}
