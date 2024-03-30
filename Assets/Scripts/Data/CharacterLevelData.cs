using System;

public class CharacterLevelData
{
    public CharacterLevelInfo[] items;

    public CharacterLevelData(string[,] csvGrid)
    {
        items = new CharacterLevelInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new CharacterLevelInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }

}

[Serializable]
public class CharacterLevelInfo
{
    public string id;
    public int levelType;
    public int level;
    public int requiredExp;
    public float atkMultiple;
    public float atkSpdMultiple;
    public float defMultiple;
    public float maxHPMultiple;

    public CharacterLevelInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        levelType = int.Parse(csvGrid[idx++, rowIdx]);
        level = int.Parse(csvGrid[idx++, rowIdx]);
        requiredExp = int.Parse(csvGrid[idx++, rowIdx]);
        atkMultiple = float.Parse(csvGrid[idx++, rowIdx]);
        atkSpdMultiple = float.Parse(csvGrid[idx++, rowIdx]);
        defMultiple = float.Parse(csvGrid[idx++, rowIdx]);
        maxHPMultiple = float.Parse(csvGrid[idx++, rowIdx]);
    }
}