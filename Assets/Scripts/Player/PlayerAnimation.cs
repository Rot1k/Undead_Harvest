using System;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private const string ANIMATOR_IS_WALKING = "IsWalking";
    private const string ANIMATOR_DEAD = "Dead";


    [SerializeField] private Transform _shadow;

    private Animator _animator;
    private SoundManager _soundManager;

    private Player _player;
    private bool _isDead;

    [Inject]
    public void Construct(SoundManager soundManager)
    {
        _soundManager = soundManager;
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (_isDead || _animator == null || _player?.Movement == null)
            return;

        _animator.SetBool(ANIMATOR_IS_WALKING, _player.Movement.IsWalking);
    }
    public void Initialize(Player player)
    {
        _player = player;
        _player.HealthSystem.OnDead += OnDead;
    }
    public void Dispose()
    {
        _player.HealthSystem.OnDead -= OnDead;
    }

    private void OnDead(object sender, EventArgs e)
    {
        _isDead = true;
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _animator.SetBool(ANIMATOR_IS_WALKING, false);
        _animator.SetTrigger(ANIMATOR_DEAD);
        _soundManager?.PlaySound(SoundType.ONDEAD);
        if (_shadow != null)
            _shadow.localScale = new Vector3(1.8f, 1.4f, 1f);
    }
}
