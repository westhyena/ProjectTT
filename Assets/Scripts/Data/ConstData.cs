using System;

[Serializable]
public class ConstData
{
    public ConstInfo[] items;

    public ConstData(string[,] csvGrid)
    {
        items = new ConstInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new ConstInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }

}

[Serializable]
public class ConstInfo
{
    public string constName;
    public string value;

    public ConstInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        constName = csvGrid[idx++, rowIdx];
        value = csvGrid[idx++, rowIdx];
    }
}
