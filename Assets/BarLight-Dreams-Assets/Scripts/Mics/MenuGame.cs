using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGame : MonoBehaviour
{
    [SerializeField] private string sceneName = "GamePlay";

    [SerializeField] private AudioClip musicGame;

    private void Start()
    {
        Time.timeScale = 1;

        AudioManager.instance.PlayMusic(musicGame);
    }

    public void StartNewGame()
    {
        int slot = SaveManager.instance.GetEmptySlot();

        if (slot == -1)
        {
            return;
        }

        SaveManager.instance.StartNewGame(slot);

        SceneTransition.instance.FadeOut(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
