using UnityEngine;

public class ResourceManager
{
    public static GameObject GetCharacterPrefab(string name)
    {
        return Resources.Load<GameObject>($"Character/{name}");
    }
}
