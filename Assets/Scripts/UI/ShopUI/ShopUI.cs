using System;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private PlayerLevelSystem _playerLevelSystem;
    [SerializeField] private WeaponsInventoryUI _weaponsInventoryUI;

    private LevelSystem LevelSystem => _playerLevelSystem.LevelSystem;
    private bool _waveEnded = false;

    private void Awake()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        WaveEndWindow.Instance.OnWindowHidden += OnWaveEnded;
        LevelSystem.OnSkillPointUsed += OnSkillPointsChanged;
        gameObject.SetActive(false);
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
            if (WavesManager.Instance.IsAllWavesCompleted == false)
            {
                Show();
            }
        }
    }

    public void Show()
    {
        _waveEnded = false;
        gameObject.SetActive(true);
        for (int i = 0; i < EquipmentManager.Instance.MaxWeapons; i++)
        {
            _weaponsInventoryUI.UpdateUI(i, EquipmentManager.Instance.GetWeapon(i));
        }
    }

    private void OnCloseButtonClicked()
    {
        gameObject.SetActive(false);
        WavesManager.Instance.StartNextWave();
    }
}
