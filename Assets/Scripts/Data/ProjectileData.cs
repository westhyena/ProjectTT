using System;

[Serializable]
public class ProjectileData
{
    public ProjectileInfo[] items;

    public ProjectileData(string[,] csvGrid)
    {
        items = new ProjectileInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new ProjectileInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }

}

[Serializable]
public class ProjectileInfo
{
    public string id;
    public string memo;
    public string projectilePrefab;
    public float scale;
    public float speed;
    public string startingType;

    public ProjectileInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        memo = csvGrid[idx++, rowIdx];
        projectilePrefab = csvGrid[idx++, rowIdx];
        scale = float.Parse(csvGrid[idx++, rowIdx]);
        speed = float.Parse(csvGrid[idx++, rowIdx]);
        startingType = csvGrid[idx++, rowIdx];
    }
}
