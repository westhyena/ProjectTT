using TMPro;
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

    CharacterDataElement characterInfo;
    public CharacterDataElement CharacterInfo => characterInfo;

    [SerializeField]
    GameObject summonEffect;

    public void Initialize(CharacterDataElement characterInfo)
    {
        this.characterInfo = characterInfo;
        image.sprite = ResourceManager.GetCharacterIcon(characterInfo.iconFileName);
        needPointText.text = 20.ToString();
        levelText.text = 1.ToString();
    }

    void Update()
    {
        int count = 0;
        HeroManager.instance.HeroList.ForEach(hero =>
        {
            if (hero.CharacterInfo.ID == characterInfo.ID)
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
