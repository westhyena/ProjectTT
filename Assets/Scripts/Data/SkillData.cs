using System;
using UnityEngine;

[Serializable]
public class SkillData
{
    public SkillInfo[] items;
    public SkillData(string[,] csvGrid)
    {
        items = new SkillInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new SkillInfo(csvGrid, i + 1);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"CharacterData Error: {e.Message}");
                items[i] = null;
            }
        }
    }    
}

[Serializable]
public class SkillInfo
{
    public string id;
    public string memo;
    public string skillAttribute;
    public bool isRangeAttack;
    public float coolDown;
    public string useTarget;
    public string effectsTarget;
    public float rangeOfEffects;
    public string projectileID;
    public string skillEffects01;
    public float effectsValue01;
    public float duration01;
    public string skillEffects02;
    public float effectsValue02;
    public float duration02;
    public string skillEffects03;
    public float effectsValue03;
    public float duration03;
    public string atkAnimation;
    public string atkedVFX;
    public string rangeVFX;
    public string iconSprite;
    
    public SkillInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        memo = csvGrid[idx++, rowIdx];
        skillAttribute = csvGrid[idx++, rowIdx];
        isRangeAttack = bool.Parse(csvGrid[idx++, rowIdx]);
        coolDown = float.Parse(csvGrid[idx++, rowIdx]);
        useTarget = csvGrid[idx++, rowIdx];
        effectsTarget = csvGrid[idx++, rowIdx];
        rangeOfEffects = float.Parse(csvGrid[idx++, rowIdx]);
        projectileID = csvGrid[idx++, rowIdx];
        skillEffects01 = csvGrid[idx++, rowIdx];
        effectsValue01 = float.Parse(csvGrid[idx++, rowIdx]);
        duration01 = float.Parse(csvGrid[idx++, rowIdx]);
        skillEffects02 = csvGrid[idx++, rowIdx];
        effectsValue02 = float.Parse(csvGrid[idx++, rowIdx]);
        duration02 = float.Parse(csvGrid[idx++, rowIdx]);
        skillEffects03 = csvGrid[idx++, rowIdx];
        effectsValue03 = float.Parse(csvGrid[idx++, rowIdx]);
        duration03 = float.Parse(csvGrid[idx++, rowIdx]);
        atkAnimation = csvGrid[idx++, rowIdx];
        atkedVFX = csvGrid[idx++, rowIdx];
        rangeVFX = csvGrid[idx++, rowIdx];
        iconSprite = csvGrid[idx++, rowIdx];
    }
}
