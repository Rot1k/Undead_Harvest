using System.Collections.Generic;

public static class EnemyRegistry
{
    private static readonly List<Enemy> _enemies = new();

    public static void Register(Enemy e)
    {
        if (e != null && !_enemies.Contains(e))
        {
            _enemies.Add(e);
        }
    }

    public static void Unregister(Enemy e)
    {
        if (e != null)
        {
            _enemies.Remove(e);
        }
    }

    public static int CountByType(EnemyStatsSO type)
    {
        int count = 0;
        for (int i = 0; i < _enemies.Count; i++)
        {
            var e = _enemies[i];
            if (e != null && !e.IsDead && e.gameObject.activeInHierarchy && e.EnemyStats == type)
            {
                count++;
            }
        }
        return count;
    }

    public static List<Enemy> GetAll() => _enemies;

    public static int GetAliveCount()
    {
        int count = 0;
        for (int i = 0; i < _enemies.Count; i++)
        {
            var e = _enemies[i];
            if (e != null && !e.IsDead && e.gameObject.activeInHierarchy)
            {
                count++;
            }
        }
        return count;
    }
}
