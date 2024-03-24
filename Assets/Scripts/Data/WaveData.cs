using System;

[Serializable]
public class WaveData
{
    public WaveInfo[] items;

    public WaveData(string[,] csvGrid)
    {
        items = new WaveInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new WaveInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }
}

[Serializable]
public class WaveInfo
{
    public string id;
    public float startTime;
    public string monsterId;
    public int totalCount;

    public WaveInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        startTime = float.Parse(csvGrid[idx++, rowIdx]);
        monsterId = csvGrid[idx++, rowIdx];
        totalCount = int.Parse(csvGrid[idx++, rowIdx]);
    }
}