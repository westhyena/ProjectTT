
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField]
    Image image;

    Player player;

    [SerializeField]
    TMP_Text levelText;

    void Start()
    {
        player = GameManager.instance.Player;
        image.sprite = ResourceManager.GetCharacterIcon(player.CharacterInfo.iconFileName);
    }

    void Update()
    {
        levelText.text = (GameManager.instance.PlayerLevel + 1).ToString();
    }
}
