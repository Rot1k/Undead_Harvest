using UnityEngine;

public class LevelUpWindowUI : MonoBehaviour
{
    [field: SerializeField] public PlayerLevelSystem PlayerLevelSystem { get; private set; }
    [SerializeField] private LevelUpPanelsController _levelUpPanelsController;

    private void Awake()
    {
        WaveEndWindow.Instance.OnWindowHidden += OnWaveCompleted;
        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void OnWaveCompleted()
    {
        TryRerollPanels();
    }
    public void TryRerollPanels()
    {
        if(WavesManager.Instance.IsAllWavesCompleted)
        {
            Hide();
            return;
        }
        if (PlayerLevelSystem.LevelSystem.SkillPoints <= 0)
        {
            Hide();
            return;
        }
        Show();
        _levelUpPanelsController.RerollPanels();
    }
}
