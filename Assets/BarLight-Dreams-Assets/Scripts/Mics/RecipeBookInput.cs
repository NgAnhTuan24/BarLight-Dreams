using UnityEngine;

public class RecipeBookInput : MonoBehaviour
{
    [SerializeField] private UIPopup recipeBookPopup;
    [SerializeField] private KeyCode recipeKey = KeyCode.C;

    void Update()
    {
        if (Input.GetKeyDown(recipeKey))
        {
            recipeBookPopup.Toggle();
        }
    }
}
