using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class TutorialUI : MonoBehaviour
{
    private const string DontShowTutorialKey = "Don'tShowTutorial";

    [SerializeField] private Button _closeButton;
    [SerializeField] private Toggle _dontShowTutorialToggle;
    [SerializeField] private bool _debugAlwaysShow;

    private PauseManager _pauseManager;

    [Inject]
    public void Construct(PauseManager pauseManager)
    {
        _pauseManager = pauseManager;
    }

    private void Awake()
    {
        _closeButton.onClick.AddListener(CloseTutorial);
        _dontShowTutorialToggle.onValueChanged.AddListener(SetDontShowTutorial);
    }

    public void Initialize()
    {
        bool dontShowTutorial = PlayerPrefs.GetInt(DontShowTutorialKey, 0) == 1;
        _dontShowTutorialToggle.SetIsOnWithoutNotify(dontShowTutorial);

        if (!_debugAlwaysShow && dontShowTutorial)
        {
            CloseTutorial();
            return;
        }

        gameObject.SetActive(true);
        _pauseManager.Pause();
    }

    private void CloseTutorial()
    {
        gameObject.SetActive(false);
        _pauseManager.Unpause();
    }

    private void SetDontShowTutorial(bool value)
    {
        PlayerPrefs.SetInt(DontShowTutorialKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}
