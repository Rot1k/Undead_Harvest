using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Collider2D))]
public class PlayerDamageReceiver : MonoBehaviour
{
    [SerializeField] private float _damageInterval = 1f; 

    private Player _player;
    private HealthSystem _healthSystem;
    private Collider2D _damageReceiveCollider;
    private readonly Dictionary<Collider2D, Coroutine> _activeDamageCoroutines = new();

    private void Awake()
    {
        _damageReceiveCollider = GetComponent<Collider2D>();
    }
    public void Initialize(Player player)
    {
        _player = player;
        _healthSystem = _player.HealthSystem;
        _healthSystem.OnDead += OnDead;
    }
    public void Dispose()
    {
        _healthSystem.OnDead -= OnDead;
    }
    private void OnDisable()
    {
        StopActiveDamageCoroutines();
    }

    private void StopActiveDamageCoroutines()
    {
        foreach (var coroutine in _activeDamageCoroutines.Values)
        {
            StopCoroutine(coroutine);
        }
        _activeDamageCoroutines.Clear();
    }

    private void OnDead(object sender, System.EventArgs e)
    {
        _damageReceiveCollider.enabled = false;
        StopActiveDamageCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_damageReceiveCollider.IsTouching(other))
            return;

        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            int damageAmount = enemy.GetDamage();

            if (!_activeDamageCoroutines.ContainsKey(other))
            {
                Coroutine coroutine = StartCoroutine(ApplyDamageOverTime(other, damageAmount));
                _activeDamageCoroutines.Add(other, coroutine);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_activeDamageCoroutines.TryGetValue(other, out var coroutine))
        {
            StopCoroutine(coroutine);
            _activeDamageCoroutines.Remove(other);
        }
    }

    private IEnumerator ApplyDamageOverTime(Collider2D enemyCollider, int damageAmount)
    {
        while (true)
        {
            _player.HealthSystem.Damage(damageAmount);
            Debug.Log($"Player Health: {_healthSystem.Health} / {_healthSystem.HealthMax}");
            yield return new WaitForSeconds(_damageInterval);
        }
    }
}
