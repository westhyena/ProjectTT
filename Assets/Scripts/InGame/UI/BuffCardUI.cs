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

    void Awake()
    {
        button = GetComponent<Button>();
        cardImage = GetComponent<Image>();
    }
}
