using TMPro;
using UnityEngine;

public class InteractionUIText : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text messageKeyText;

    private Transform currentAnchor;

    private void Awake()
    {
        Hide();
    }

    private void LateUpdate()
    {
        if (currentAnchor == null) return;

        transform.position = currentAnchor.position;
    }

    public void Show(Transform anchor, KeyCode key)
    {
        currentAnchor = anchor;

        messageKeyText.text = $"Press {key}";

        transform.position = anchor.position;

        root.SetActive(true);
    }

    public void Hide()
    {
        currentAnchor = null;

        if (root != null)
            root.SetActive(false);
    }
}