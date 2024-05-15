using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BuffCardUI : MonoBehaviour
{
    public Button button;

    public Image cardImage;

    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    UserSelectCardDataElement cardData;
    public UserSelectCardDataElement CardData => cardData;

    void Awake()
    {
        button = GetComponent<Button>();
        cardImage = GetComponent<Image>();
    }

    public void Initialize(UserSelectCardDataElement cardData)
    {
        this.cardData = cardData;
        iconImage.sprite = ResourceManager.GetBuffIcon(cardData.CardIconName);
        nameText.text = cardData.CardName;
        descriptionText.text = cardData.CardName;

        cardImage.sprite = UIManager.instance.buffGradeIcon[(int)cardData.CardRating];
    }
}
