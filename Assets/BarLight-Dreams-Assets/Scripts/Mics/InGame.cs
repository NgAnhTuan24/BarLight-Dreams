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

    public void RetryDay()
    {
        int slot = SaveManager.instance.CurrentSlot;

        if (slot > 0 && SaveLoadSystem.HasSave(slot))
        {
            SaveManager.instance.StartLoadGame(slot);

            SceneTransition.instance.FadeOut(() =>
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
