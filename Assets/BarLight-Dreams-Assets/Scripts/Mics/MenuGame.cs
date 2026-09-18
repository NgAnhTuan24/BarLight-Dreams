using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuGame : MonoBehaviour
{
    [SerializeField] private string sceneName = "GamePlay";
    [SerializeField] private TMP_Text versionText;

    [SerializeField] private AudioClip musicGame;

    [SerializeField] private SceneTransition fadeIn;
    [SerializeField] private SceneTransition fadeOut;

    [SerializeField] private Button newGameButton;

    private void Start()
    {
        Time.timeScale = 1;

        versionText.text = $"DEMO v{Application.version}";

        AudioManager.instance.PlayMusic(musicGame);

        fadeIn.FadeIn();

        RefreshNewGameButton();
    }

    private void RefreshNewGameButton() 
    { 
        bool hasEmptySlot = SaveManager.instance.GetEmptySlot() != -1; 
        
        newGameButton.interactable = hasEmptySlot; 
    }

    public void StartNewGame()
    {
        int slot = SaveManager.instance.GetEmptySlot();

        if (slot == -1)
        {
            return;
        }

        SaveManager.instance.StartNewGame(slot);

        fadeOut.FadeOut(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
