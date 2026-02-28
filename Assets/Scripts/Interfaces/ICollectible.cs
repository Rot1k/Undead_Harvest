using UnityEngine;

public interface ICollectible
{
    void Collect(ItemCollector collector);
    void StartAttraction(Transform target, float speed);
}
