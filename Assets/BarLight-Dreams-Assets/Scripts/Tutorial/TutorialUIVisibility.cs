using UnityEngine;

public class TutorialObjectHider : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToShowAfterTutorial;

    private void Start()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnTutorialCompleted += HandleTutorialCompleted;
        }
    }

    private void OnDestroy()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnTutorialCompleted -= HandleTutorialCompleted;
        }
    }

    private void HandleTutorialCompleted()
    {
        foreach (GameObject obj in objectsToShowAfterTutorial)
        {
            if (obj == null)
                continue;

            obj.SetActive(true);
        }
    }
}