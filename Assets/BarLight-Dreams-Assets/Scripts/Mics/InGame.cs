using UnityEngine;
using UnityEngine.SceneManagement;

public class InGame : MonoBehaviour
{
    [SerializeField] private AudioClip musicInGame;

    [SerializeField] private SceneTransition sceneTransition;

    void Start()
    {
        AudioManager.instance.PlayMusic(musicInGame);
    }

    public void RetryDay()
    {
        int slot = SaveManager.instance.CurrentSlot;

        if (slot > 0 && SaveLoadSystem.HasSave(slot))
        {
            SaveManager.instance.StartLoadGame(slot);

            sceneTransition.FadeOut(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
