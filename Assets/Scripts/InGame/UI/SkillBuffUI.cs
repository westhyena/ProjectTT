using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillBuffUI : MonoBehaviour
{
    public GameObject buffPrefab;

    public Transform buffParent;
    public Transform debuffParent;

    Character character;

    List<Image> buffImages = new List<Image>();
    List<Image> debuffImages = new List<Image>();

    public void Initialize(Character character)
    {
        this.character = character;
    }

    void Update()
    {
        List<BuffIcons> buffIcons = new List<BuffIcons>();
        List<BuffIcons> debuffIcons = new List<BuffIcons>();
        foreach (Skill.EffectHolder effectHolder in character.SkillEffectList)
        {
            buffIcons.AddRange(effectHolder.skill.SkillInfo.BuffIcons);
            debuffIcons.AddRange(effectHolder.skill.SkillInfo.deBuffIcons);
        }

        for (int i = 0; i < buffIcons.Count; ++i)
        {
            if (buffImages.Count < buffIcons.Count)
            {
                GameObject buffObj = Instantiate(buffPrefab, buffParent);
                Image buffImage = buffObj.GetComponent<Image>();
                buffImages.Add(buffImage);
            }

            buffImages[i].gameObject.SetActive(true);
            buffImages[i].sprite = ResourceManager.GetIngameBuffDebuffIcon(buffIcons[i].IconFileName);
        }

        for (int i = 0; i < debuffIcons.Count; ++i)
        {
            if (debuffImages.Count < debuffIcons.Count)
            {
                GameObject debuffObj = Instantiate(buffPrefab, debuffParent);
                Image debuffImage = debuffObj.GetComponent<Image>();
                debuffImages.Add(debuffImage);
            }

            debuffImages[i].gameObject.SetActive(true);
            debuffImages[i].sprite = ResourceManager.GetIngameBuffDebuffIcon(debuffIcons[i].IconFileName);
        }

        for (int i = buffIcons.Count; i < buffImages.Count; ++i)
        {
            buffImages[i].gameObject.SetActive(false);
        }
        for (int i = debuffIcons.Count; i < debuffImages.Count; ++i)
        {
            debuffImages[i].gameObject.SetActive(false);
        }
    }

}
