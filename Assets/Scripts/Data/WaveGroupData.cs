using System;

[Serializable]
public class WaveGroupData
{
    public WaveGroupInfo[] items;

    public WaveGroupData(string[,] csvGrid)
    {
        items = new WaveGroupInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new WaveGroupInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }
}

[Serializable]
public class WaveGroupInfo
{
    public string id;
    public string wave01;
    public string wave02;
    public string wave03;
    public string wave04;

    public WaveGroupInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        wave01 = csvGrid[idx++, rowIdx];
        wave02 = csvGrid[idx++, rowIdx];
        wave03 = csvGrid[idx++, rowIdx];
        wave04 = csvGrid[idx++, rowIdx];
    }
}
