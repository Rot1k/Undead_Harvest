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

    private PlayerMovement _playerMovement;
    private PlayerHealthSystem _playerHealthSystem;

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
        _animator.SetBool(ANIMATOR_IS_WALKING, _playerMovement.IsWalking);
    }
    public void Initialize(PlayerMovement playerMovement, PlayerHealthSystem playerHealthSystem)
    {
        _playerMovement = playerMovement;
        _playerHealthSystem = playerHealthSystem;
        _playerHealthSystem.HealthSystem.OnDead += OnDead;
    }
    public void Dispose()
    {
        _playerHealthSystem.HealthSystem.OnDead -= OnDead;
    }

    private void OnDead(object sender, EventArgs e)
    {
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        _animator.SetTrigger(ANIMATOR_DEAD);
        _soundManager?.PlaySound(SoundType.ONDEAD);
        _shadow.localScale = new Vector3(1.8f, 1.4f, 1);
    }
}
