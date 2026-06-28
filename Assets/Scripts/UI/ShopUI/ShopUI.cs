using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private PlayerLevelSystem _playerLevelSystem;
    private LevelSystem LevelSystem;
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

    public void Initialize(PlayerLevelSystem playerLevelSystem)
    {
        _playerLevelSystem = playerLevelSystem;
        LevelSystem = _playerLevelSystem.LevelSystem;

        if (_waveEndWindow != null)
        {
            _waveEndWindow.OnWindowHidden += OnWaveEnded;
        }
        else
        {
            Debug.LogWarning("ShopUI.Initialize: WaveEndWindow not available when initializing ShopUI.");
        }

        if (LevelSystem != null)
        {
            LevelSystem.OnSkillPointUsed += OnSkillPointsChanged;
        }

        gameObject.SetActive(false);
    }

    public void Dispose()
    {
        if (_waveEndWindow != null)
            _waveEndWindow.OnWindowHidden -= OnWaveEnded;
        if (LevelSystem != null)
            LevelSystem.OnSkillPointUsed -= OnSkillPointsChanged;
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }

    private void OnDestroy()
    {
        Dispose();
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
