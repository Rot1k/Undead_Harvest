using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using VContainer;

public class ExpBarUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Image _expFillImage;

    private LevelSystem _levelSystem;

    [Inject]
    public void Construct(Player player)
    {
        _levelSystem = player.Level.LevelSystem;
    }

    public void Initialize()
    {
        var currentExp = _levelSystem.CurrentExp;
        var expToNextLevel = _levelSystem.ExpToNextLevel;


        Observable.CombineLatest(
            currentExp,
            expToNextLevel,
            (exp, nextLevelExp) => (exp, nextLevelExp))
            .Subscribe(UpdateExpUI)
            .AddTo(this);

        _levelSystem.Level.Subscribe(level => UpdateLevelUI(level))
            .AddTo(this);
    }

    private void UpdateLevelUI(int level)
    {
        _levelText.text = $"LV. {level}";
    }
    private void UpdateExpUI((float exp, float nextLevelExp) values)
    {
        _expFillImage.fillAmount = values.exp / values.nextLevelExp;
    }
}
