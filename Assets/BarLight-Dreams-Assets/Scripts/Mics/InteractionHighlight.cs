using UnityEngine;

public class InteractionHighlight : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    [SerializeField] private UIPopup targetPopup;
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    [SerializeField] private InteractionUIText interactionUI;
    [SerializeField] private Transform textAnchor;

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

    [Header("Pickup Drink")]
    [SerializeField] private PickupCounter pickupCounter;

    private bool playerInRange;
    private bool canShowTextUI;

    private void Start()
    {
        if (UIManager.Instance != null && UIManager.Instance.IsGameplayInputLocked) return;

        if (highlight != null)
        {
            highlight.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        bool newCanShowTextUI = CanInteract();

        if (newCanShowTextUI != canShowTextUI)
        {
            canShowTextUI = newCanShowTextUI;

            if (canShowTextUI)
                interactionUI?.Show(textAnchor, interactKey);
            else
                interactionUI?.Hide();
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (pickupCounter != null)
            {
                if (pickupCounter.TryPlaceDrink())
                    return;
            }

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

            if (drinkMixer != null && drinkMixer.CanMix())
            {
                drinkMixer.StartMixing();
            }
        }
    }

    private bool CanInteract()
    {
        if (drinkMixer != null)
        {
            return drinkMixer.CanMix();
        }

        if (pickupCounter != null)
        {
            return PlayerHoldItem.instance.HasDrink() && pickupCounter.HasSpace();
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            canShowTextUI = false;

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
            canShowTextUI = false;

            if (highlight != null)
            {
                highlight.SetActive(false);
            }

            interactionUI?.Hide();

            if (drinkMixer != null) return;

            if (targetPopup != null && targetPopup.IsOpen)
            {
                targetPopup.Close();
            }
        }
    }
}
