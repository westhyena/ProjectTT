using UnityEngine;

public class ResourceManager
{
    public static GameObject GetCharacterPrefab(string name)
    {
        return Resources.Load<GameObject>($"Character/{name}");
    }

    public static Sprite GetCharacterIcon(string name)
    {
        return Resources.Load<Sprite>($"CharacterIcon/{name}");
    }

    public static GameObject GetSkillPrefab(string name)
    {
        return Resources.Load<GameObject>($"Skill/{name}");
    }

    public static GameObject GetStagePrefab(string name)
    {
        return Resources.Load<GameObject>($"Stage/{name}");
    }
}
