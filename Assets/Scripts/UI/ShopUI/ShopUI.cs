using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private PlayerLevelSystem _playerLevelSystem;
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private LevelSystem LevelSystem => _playerLevelSystem.LevelSystem;
    private bool _waveEnded = false;

    private WavesManager _wavesManager;
    private WaveEndWindow _waveEndWindow;
    private EquipmentManager _equipmentManager;

    [Inject]
    public void Construct(WavesManager wavesManager, WaveEndWindow waveEndWindow, EquipmentManager equipmentManager)
    {
        _wavesManager = wavesManager;
        _waveEndWindow = waveEndWindow;
        _equipmentManager = equipmentManager;
    }

    private void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
    }

    private void Start()
    {
        _waveEndWindow.OnWindowHidden += OnWaveEnded;
        LevelSystem.OnSkillPointUsed += OnSkillPointsChanged;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_waveEndWindow != null)
            _waveEndWindow.OnWindowHidden -= OnWaveEnded;

        if (LevelSystem != null)
            LevelSystem.OnSkillPointUsed -= OnSkillPointsChanged;
    }

    private void OnWaveEnded()
    {
        _waveEnded = true;
        TryShow();
    }

    private void OnSkillPointsChanged(object sender, EventArgs e)
    {
        TryShow();
    }

    private void TryShow()
    {
        if (_waveEnded && LevelSystem.SkillPoints == 0)
        {
            if (_wavesManager.IsAllWavesCompleted == false)
            {
                Show();
            }
        }
    }

    public void Show()
    {
        _waveEnded = false;
        gameObject.SetActive(true);
        for (int i = 0; i < _equipmentManager.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, _equipmentManager.GetWeapon(i));
        }
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
        _wavesManager.StartNextWave();
    }
}
