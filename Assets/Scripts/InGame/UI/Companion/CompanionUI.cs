using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompanionUI : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    TMP_Text needPointText;

    [SerializeField]
    TMP_Text levelText;

    [SerializeField]
    TMP_Text countText;

    CharacterInfo characterInfo;
    public CharacterInfo CharacterInfo => characterInfo;

    [SerializeField]
    GameObject summonEffect;

    public void Initialize(CharacterInfo characterInfo)
    {
        this.characterInfo = characterInfo;
        image.sprite = ResourceManager.GetCharacterIcon(characterInfo.iconSprite);
        needPointText.text = 20.ToString();
        levelText.text = 1.ToString();
    }

    void Update()
    {
        int count = 0;
        HeroManager.instance.HeroList.ForEach(hero =>
        {
            if (hero.CharacterInfo.id == characterInfo.id)
            {
                count++;
            }
        });

        countText.text = count.ToString();
    }

    public void CreateSummonEffect()
    {
        GameObject obj = Instantiate(summonEffect, transform.position, Quaternion.identity, this.transform);
        obj.transform.localPosition = Vector3.zero;
    }
}
