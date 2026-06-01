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

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
