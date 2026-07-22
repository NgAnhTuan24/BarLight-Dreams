using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private int gameplayInputLockCount;
    private int pauseInputLockCount;

    public bool IsGameplayInputLocked => gameplayInputLockCount > 0;
    public bool IsPauseInputLocked => pauseInputLockCount > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #region Gameplay Input

    public void LockGameplayInput()
    {
        gameplayInputLockCount++;
    }

    public void UnlockGameplayInput()
    {
        gameplayInputLockCount = Mathf.Max(0, gameplayInputLockCount - 1);
    }

    #endregion

    #region Pause Input

    public void LockPauseInput()
    {
        pauseInputLockCount++;
    }

    public void UnlockPauseInput()
    {
        pauseInputLockCount = Mathf.Max(0, pauseInputLockCount - 1);
    }

    #endregion
}