using System;

[Serializable]
public class EffectData
{
    public EffectInfo[] items;
    public EffectData(string[,] csvGrid)
    {
        items = new EffectInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new EffectInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }
}

[Serializable]
public class EffectInfo
{
    public string id;
    public string memo;
    public string effectType;
    public string basedStat;
    public string applyType;
    public string applyStat;
    public float repeatTime;

    public EffectInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        memo = csvGrid[idx++, rowIdx];
        effectType = csvGrid[idx++, rowIdx];
        basedStat = csvGrid[idx++, rowIdx];
        applyType = csvGrid[idx++, rowIdx];
        applyStat = csvGrid[idx++, rowIdx];
        repeatTime = float.Parse(csvGrid[idx++, rowIdx]);
    }
}
