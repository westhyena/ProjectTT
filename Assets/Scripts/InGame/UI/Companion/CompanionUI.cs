using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionUI : MonoBehaviour
{
    Button button;

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

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(this.OnClick);
    }

    void OnClick()
    {
        int level = GameManager.instance.GetCompanionLevel(characterInfo.ID);
        List<int> needPointList = DataMgr.instance.m_InGameSystemElement.Mercenary_LevelUp_NeedPoint;
        if (level >= needPointList.Count)
        {
            return;
        }

        int needPoint = needPointList[level];
        if (GameManager.instance.CompanionPoints >= needPoint)
        {
            GameManager.instance.CompanionLevelUp(characterInfo.ID);
        }
    }

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
        HeroManager.instance.AliveHeroList.ForEach(hero =>
        {
            if (hero.CharacterInfo.ID == characterInfo.ID)
            {
                count++;
            }
        });

        countText.text = count.ToString();

        int level = GameManager.instance.GetCompanionLevel(characterInfo.ID);
        levelText.text = (level + 1).ToString();

        List<int> needPointList = DataMgr.instance.m_InGameSystemElement.Mercenary_LevelUp_NeedPoint;
        if (level >= needPointList.Count)
        {
            needPointText.text = "MAX";
        } 
        else
        {
            int needPoint = needPointList[level];
            needPointText.text = needPoint.ToString();
        }
    }

    public void CreateSummonEffect()
    {
        GameObject obj = Instantiate(summonEffect, transform.position, Quaternion.identity, this.transform);
        obj.transform.localPosition = Vector3.zero;
    }
}
