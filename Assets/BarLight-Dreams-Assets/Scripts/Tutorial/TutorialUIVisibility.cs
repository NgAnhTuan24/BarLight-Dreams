using UnityEngine;

public class TutorialObjectHider : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToShowAfterTutorial;

    private void Start()
    {
        if (TutorialManager.instance == null)
            return;

        TutorialManager.instance.OnTutorialStateChanged += RefreshObjects;

        RefreshObjects();
    }

    private void OnDestroy()
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.OnTutorialCompleted -= RefreshObjects;
        }
    }

    private void RefreshObjects()
    {
        bool showObjects = TutorialManager.instance.IsTutorialCompleted;

        foreach (GameObject obj in objectsToShowAfterTutorial)
        {
            if (obj == null)
                continue;

            obj.SetActive(showObjects);
        }
    }
}