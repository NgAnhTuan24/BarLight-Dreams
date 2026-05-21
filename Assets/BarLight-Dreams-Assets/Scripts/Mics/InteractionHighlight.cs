using UnityEngine;

public class InteractionHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    [SerializeField] private UIPopup targetPopup;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Remove All In Counter")]
    [SerializeField] private bool cleanCounter;

    [Header("Give Ice")]
    [SerializeField] private bool giveIce;
    [SerializeField] private Sprite iceSprite;

    [Header("Give Cup")]
    [SerializeField] private bool giveCup;
    [SerializeField] private Sprite cupSprite;


    private bool playerInRange;

    private void Start()
    {
        highlight.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(interactKey))
        {
            if (targetPopup != null)
            {
                targetPopup.Toggle();
            }

            if (cleanCounter)
            {
                CounterBarUI.instance.CleanCounter();
                PlayerHoldItem.instance.Clear();
            }

            if (giveIce)
            {
                CounterBarUI.instance.AddIngredient(
                    iceSprite,
                    new Vector2 (40, 50),
                    17.5f
                    );
            }

            if (giveCup)
            {
                if (!PlayerHoldItem.instance.HasItem)
                {
                    PlayerHoldItem.instance.Hold(cupSprite);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            highlight.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            highlight.SetActive(false);

            if (targetPopup != null && targetPopup.IsOpen)
            {
                targetPopup.Close();
            }
        }
    }
}
