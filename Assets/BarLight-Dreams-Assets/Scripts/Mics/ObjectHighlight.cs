using UnityEngine;

public class ObjectHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    private void Start()
    {
        highlight.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            highlight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            highlight.SetActive(false);
        }
    }
}
