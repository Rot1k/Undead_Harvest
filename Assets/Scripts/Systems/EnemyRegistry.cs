using System.Collections.Generic;

public sealed class EnemyRegistry
{
    private readonly List<Enemy> _enemies = new();

    public void Register(Enemy e)
    {
        if (e != null && !_enemies.Contains(e))
        {
            _enemies.Add(e);
        }
    }

    public void Unregister(Enemy e)
    {
        if (e != null)
        {
            _enemies.Remove(e);
        }
    }

    public int CountByType(EnemyStatsSO type)
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

    public IReadOnlyList<Enemy> GetAll() => _enemies;

    public int GetAliveCount()
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
