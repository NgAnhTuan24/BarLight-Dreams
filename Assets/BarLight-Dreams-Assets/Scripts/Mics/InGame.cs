using UnityEngine;

public class InGame : MonoBehaviour
{
    [SerializeField] private AudioClip musicInGame;

    void Start()
    {
        AudioManager.instance.PlayMusic(musicInGame);
    }
}
