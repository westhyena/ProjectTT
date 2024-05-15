using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class BuffCardUI : MonoBehaviour
{
    public Button button;

    public Image cardImage;

    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    void Awake()
    {
        button = GetComponent<Button>();
        cardImage = GetComponent<Image>();
    }

    public void Initialize(UserSelectCardDataElement cardData)
    {
        iconImage.sprite = ResourceManager.GetBuffIcon(cardData.CardIconName);
        nameText.text = cardData.CardName;
        descriptionText.text = cardData.CardName;

        cardImage.sprite = UIManager.instance.buffGradeIcon[(int)cardData.CardRating];
    }
}
