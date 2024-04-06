using System;

[Serializable]
public class AttackTypeData
{
    public AttackTypeInfo[] items;
    public AttackTypeData(string[,] csvGrid)
    {
        items = new AttackTypeInfo[csvGrid.GetUpperBound(1) - 1];
        for (int i = 0; i < items.Length; ++i)
        {
            try
            {
                items[i] = new AttackTypeInfo(csvGrid, i + 1);
            }
            catch (Exception)
            {
                items[i] = null;
            }
        }
    }
}

[Serializable]
public class AttackTypeInfo
{
    public string id;
    public string memo;
    public string atkAttribute;
    public bool isTargetToGround;
    public bool isTargetToAir;
    public float atkWeight;
    public float atkSpdWeight;
    public float hpWeight;
    public float defWeight;

    public AttackTypeInfo(string[,] csvGrid, int rowIdx)
    {
        int idx = 0;
        id = csvGrid[idx++, rowIdx];
        memo = csvGrid[idx++, rowIdx];
        atkAttribute = csvGrid[idx++, rowIdx];
        isTargetToGround = bool.Parse(csvGrid[idx++, rowIdx]);
        isTargetToAir = bool.Parse(csvGrid[idx++, rowIdx]);
        atkWeight = float.Parse(csvGrid[idx++, rowIdx]);
        atkSpdWeight = float.Parse(csvGrid[idx++, rowIdx]);
        hpWeight = float.Parse(csvGrid[idx++, rowIdx]);
        defWeight = float.Parse(csvGrid[idx++, rowIdx]);
    }

}
