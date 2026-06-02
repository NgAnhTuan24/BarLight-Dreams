using UnityEngine;

public class GameOverUIHandler : MonoBehaviour
{
    public static GameOverUIHandler instance;

    public UIPopup gameOverUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
