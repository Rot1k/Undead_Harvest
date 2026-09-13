using System;
using UnityEngine;
using VContainer;

public class LevelUpWindowUI : MonoBehaviour
{
    public event Action OnCompleted;

    private Player _player;
    public Player Player => _player;
    [SerializeField] private LevelUpPanelsController _levelUpPanelsController;

    [Inject]
    public void Construct(Player player)
    {
        _player = player;
    }

    public void Initialize()
    {
        _levelUpPanelsController.Initialize(_player);
        _player.Level.LevelSystem.OnSkillPointUsed += OnSkillPointsChanged;
        Hide();
    }

    private void Start()
    {
        // Initialization handled in StartGame called by UIBootstrap
    }

    public void Dispose()
    {
        _player.Level.LevelSystem.OnSkillPointUsed -= OnSkillPointsChanged;
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        if (_player.Level.LevelSystem.SkillPoints <= 0)
            return;

        gameObject.SetActive(true);
        TryRerollPanels();
    }

    public void TryRerollPanels()
    {
        if (!gameObject.activeSelf || _player.Level.LevelSystem.SkillPoints <= 0)
            return;

        _levelUpPanelsController.RerollPanels();
    }

    private void OnSkillPointsChanged(object sender, EventArgs e)
    {
        if (gameObject.activeSelf && _player.Level.LevelSystem.SkillPoints <= 0)
        {
            Hide();
            OnCompleted?.Invoke();
        }
    }
}
