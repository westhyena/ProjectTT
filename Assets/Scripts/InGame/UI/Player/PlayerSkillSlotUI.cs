using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkillSlotUI : MonoBehaviour
{
    [SerializeField]
    Image iconImage;

    [SerializeField]
    Image maskImage;

    Skill skill;

    public void Initialize(Skill skill)
    {
        this.skill = skill;
        iconImage.sprite = ResourceManager.GetSkillIcon(skill.SkillInfo.IconName);
    }

    void Update()
    {
        maskImage.fillAmount = Mathf.Clamp01(1.0f - skill.CoolTimeRatio);
    }
}
