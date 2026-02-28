using UnityEngine;
using NTC.Pool;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Enemy))]
public class EnemyVisual : MonoBehaviour, ISpawnable
{
    private const string ANIMATOR_DEAD = "Dead";

    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private Transform _shadow;

    private Enemy _enemy;
    private Animator _animator;
    private HealthSystem _healthSystem;
    private Coroutine _fadeCoroutine;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _shadowSpriteRenderer;
    private Color _originalColor;
    private Color _shadowOriginalColor;
    private int _originalSortingOrder;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalColor = _spriteRenderer.color;
        _shadowSpriteRenderer = _shadow.GetComponent<SpriteRenderer>();
        _shadowOriginalColor = _shadowSpriteRenderer.color;
        _enemy = GetComponent<Enemy>();
        _healthSystem = _enemy.HealthSystem;
        _animator = GetComponent<Animator>();
        _originalSortingOrder = _spriteRenderer.sortingOrder;
    }
    void LateUpdate()
    {
        _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }

    private void OnEnable()
    {
        if (_enemy != null)
        {
            _healthSystem.OnDamaged += OnDamaged;
            _healthSystem.OnDead += OnDead;
        }
        else
        {
            Debug.LogWarning("Enemy is not assigned in EnemyVisual.");
        }
    }
    private void OnDisable()
    {
        if (_enemy != null)
        {
            _healthSystem.OnDamaged -= OnDamaged;
            _healthSystem.OnDead -= OnDead;
        }
    }
    private void OnDamaged(object sender, System.EventArgs e)
    {
        if (_hitEffect != null)
            NightPool.Spawn(_hitEffect, transform.position, Quaternion.identity);
    }
    private void OnDead(object sender, System.EventArgs e)
    {
        _animator.SetBool(ANIMATOR_DEAD, true);
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
        }
        if (gameObject.activeInHierarchy)
            _fadeCoroutine = StartCoroutine(FadeOut());
        _spriteRenderer.sortingOrder = _originalSortingOrder - 1;
    }
    private IEnumerator FadeOut()
    {
        float duration = 1f;
        float time = 0f;

        Color color = _spriteRenderer.color;
        Color shadowColor = _shadowSpriteRenderer.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            float shadowAlpha = Mathf.Lerp(_shadowOriginalColor.a, 0f, time / duration);

            color.a = alpha;
            shadowColor.a = shadowAlpha;

            _spriteRenderer.color = color;
            _shadowSpriteRenderer.color = shadowColor;

            yield return null;
        }
    }
    public void OnSpawn()
    {
        _spriteRenderer.sortingOrder = _originalSortingOrder;
        _animator.SetBool(ANIMATOR_DEAD, false);
        _spriteRenderer.color = _originalColor;
        _shadowSpriteRenderer.color = _shadowOriginalColor;
    }
}
