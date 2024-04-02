using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public CharacterInfo[] items;

    public CharacterData(string[,] csvGrid)
    {
        items = new CharacterInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new CharacterInfo(csvGrid, i + 1);
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
public class CharacterInfo
{
    public string id;
    public string name;
    public float rangeOfTarget;
    public float baseMSpd;
    public float baseMaxHP;
    public float baseAttack;
    public float baseAtkSpd;
    public float baseDefense;
    public string prefabKey;
    public string iconSprite;
    public string normalAtk;
    public string[] skillIDs;
    
    public CharacterInfo(string[,] csvGrid, int rowidx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowidx];
        name = csvGrid[idx++, rowidx];
        rangeOfTarget = float.Parse(csvGrid[idx++, rowidx]);
        baseMSpd = float.Parse(csvGrid[idx++, rowidx]);
        baseMaxHP = float.Parse(csvGrid[idx++, rowidx]);
        baseAttack = float.Parse(csvGrid[idx++, rowidx]);
        baseAtkSpd = float.Parse(csvGrid[idx++, rowidx]);
        baseDefense = float.Parse(csvGrid[idx++, rowidx]);
        prefabKey = csvGrid[idx++, rowidx];
        iconSprite = csvGrid[idx++, rowidx];
        normalAtk = csvGrid[idx++, rowidx];

        string skillStr = csvGrid[idx++, rowidx];
        skillIDs = skillStr == null ? new string[0] : skillStr.Split("|");
    }
}
