using System;

[Serializable]
public class StageData
{
    public StageInfo[] items;

    public StageData(string[,] csvGrid)
    {
        items = new StageInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new StageInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }

}

[Serializable]
public class StageInfo
{
    public string id;
    public string phase01waveGroup;
    public string phase02waveGroup;
    public string phase03waveGroup;
    public string phase04waveGroup;
    public string phase05waveGroup;
    public string stagePrefab;

    public StageInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        phase01waveGroup = csvGrid[idx++, rowIdx];
        phase02waveGroup = csvGrid[idx++, rowIdx];
        phase03waveGroup = csvGrid[idx++, rowIdx];
        phase04waveGroup = csvGrid[idx++, rowIdx];
        phase05waveGroup = csvGrid[idx++, rowIdx];
        stagePrefab = csvGrid[idx++, rowIdx];
    }
}
