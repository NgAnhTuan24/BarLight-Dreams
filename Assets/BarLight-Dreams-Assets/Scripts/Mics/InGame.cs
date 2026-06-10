using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour
{
    [SerializeField] private AudioClip musicInGame;

    void Start()
    {
        AudioManager.instance.PlayMusic(musicInGame);
    }

    public void Save()
    {
        SaveManager.instance.SaveGame();
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
