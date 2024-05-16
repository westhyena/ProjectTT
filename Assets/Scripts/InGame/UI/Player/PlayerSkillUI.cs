using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillUI : MonoBehaviour
{
    Player player;

    [SerializeField]
    GameObject skillSlotPrefab;

    List<PlayerSkillSlotUI> skillSlotList = new ();

    void Start()
    {
        this.player = GameManager.instance.Player;

        foreach (Skill skill in this.player.SkillList)
        {
            GameObject skillSlotObject = Instantiate(skillSlotPrefab, transform);
            PlayerSkillSlotUI skillSlotUI = skillSlotObject.GetComponent<PlayerSkillSlotUI>();
            skillSlotUI.Initialize(skill);
            skillSlotList.Add(skillSlotUI);
        }
    }

    void Update()
    {
    }
}
