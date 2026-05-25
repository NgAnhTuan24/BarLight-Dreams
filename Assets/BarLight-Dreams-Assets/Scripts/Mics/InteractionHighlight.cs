using UnityEngine;

public class InteractionHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    [SerializeField] private UIPopup targetPopup;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Remove All In Counter")]
    [SerializeField] private bool cleanCounter;
    [SerializeField] private AudioClip cleanSFX;

    [Header("Give Ice")]
    [SerializeField] private bool giveIce;
    [SerializeField] private Sprite iceSprite;
    [SerializeField] private AudioClip iceSFX;

    [Header("Give Cup")]
    [SerializeField] private bool giveCup;
    [SerializeField] private Sprite cupSprite;
    [SerializeField] private AudioClip cupSFX;

    [Header("Mix Drink")]
    [SerializeField] private DrinkMixer drinkMixer;

    private bool playerInRange;

    private void Start()
    {
        if (highlight != null)
        {
            highlight.SetActive(false);
        }
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
                AudioManager.instance.PlaySFX(cleanSFX);
            }

            if (giveIce)
            {
                CounterBarUI.instance.AddIngredient(
                    IngredientType.Ice,
                    iceSprite,
                    new Vector2 (40, 50),
                    17.5f
                );

                AudioManager.instance.PlaySFX(iceSFX);
            }

            if (giveCup)
            {
                if (PlayerHoldItem.instance.IsEmpty())
                {
                    PlayerHoldItem.instance.Hold(cupSprite, HoldItemType.Cup);

                    AudioManager.instance.PlaySFX(cupSFX);
                }
            }

            if (drinkMixer != null)
            {
                drinkMixer.Mix();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (highlight != null)
            {
                highlight.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (highlight != null)
            {
                highlight.SetActive(false);
            }

            if (targetPopup != null && targetPopup.IsOpen)
            {
                targetPopup.Close();
            }
        }
    }
}
