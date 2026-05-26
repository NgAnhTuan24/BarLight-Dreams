using UnityEngine;
using UnityEngine.UI;

public class MixingArrowUI : MonoBehaviour
{
    [SerializeField] private Image arrowImage;

    [Header("Sprites")]
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color correctColor = Color.green;
    [SerializeField] private Color wrongColor = Color.red;

    private ArrowType arrowType;

    public ArrowType ArrowType => arrowType;

    public void Setup(ArrowType type)
    {
        arrowType = type;

        arrowImage.color = normalColor;

        switch (type)
        {
            case ArrowType.Up:
                arrowImage.sprite = upSprite;
                break;

            case ArrowType.Down:
                arrowImage.sprite = downSprite;
                break;

            case ArrowType.Left:
                arrowImage.sprite = leftSprite;
                break;

            case ArrowType.Right:
                arrowImage.sprite = rightSprite;
                break;
        }
    }

    public void SetCorrect()
    {
        arrowImage.color = correctColor;
    }

    public void SetWrong()
    {
        arrowImage.color = wrongColor;
    }

    public void ResetColor()
    {
        arrowImage.color = normalColor;
    }
}