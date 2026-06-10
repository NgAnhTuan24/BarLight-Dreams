using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGame : MonoBehaviour
{
    [SerializeField] private string sceneName = "GamePlay";

    [SerializeField] private AudioClip musicGame;

    private void Start()
    {
        AudioManager.instance.PlayMusic(musicGame);
    }

    public void StartGame() //New Game
    {
        int slot = SaveManager.instance.GetEmptySlot();

        if (slot == -1)
        {
            return;
        }

        SaveManager.instance.StartNewGame(slot);

        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
