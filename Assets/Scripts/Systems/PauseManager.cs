using UnityEngine;

public sealed class PauseManager
{
    private int _pauseCount;

    public int PauseCount => _pauseCount;
    public bool IsPaused => _pauseCount > 0;

    public PauseManager()
    {
        Time.timeScale = 1f;
    }

    public void Pause()
    {
        _pauseCount++;
        UpdatePauseState();
    }

    public void Unpause()
    {
        if (_pauseCount == 0)
            return;

        _pauseCount--;
        UpdatePauseState();
    }

    private void UpdatePauseState()
    {
        Time.timeScale = IsPaused ? 0f : 1f;
    }
}
