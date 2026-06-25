using System.Collections.Generic;

public static class EnemyRegistry
{
    private static readonly List<Enemy> _enemies = new();

    public static void Register(Enemy e) => _enemies.Add(e);
    public static void Unregister(Enemy e) => _enemies.Remove(e);

    public static int CountByType(EnemyStatsSO type)
    {
        int count = 0;
        foreach (var e in _enemies)
            if (e.EnemyStats == type)
                count++;
        return count;
    }
    public static IEnumerable<Enemy> GetAll() => _enemies;
    public static int GetAliveCount() => _enemies.Count;
}
