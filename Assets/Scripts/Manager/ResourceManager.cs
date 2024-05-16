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

    public static Sprite GetBuffIcon(string name)
    {
        return Resources.Load<Sprite>($"BuffIcon/{name}");
    }

    public static GameObject GetSkillPrefab(string name)
    {
        return Resources.Load<GameObject>($"Skill/{name}");
    }

    public static Sprite GetSkillIcon(string name)
    {
        return Resources.Load<Sprite>($"SkillIcon/{name}");
    }

    public static GameObject GetProjectilePrefab(string name)
    {
        return Resources.Load<GameObject>($"Projectile/{name}");
    }

    public static GameObject GetHitPrefab(string name)
    {
        return Resources.Load<GameObject>($"Hit/{name}");
    }

    public static GameObject GetStagePrefab(string name)
    {
        return Resources.Load<GameObject>($"Stage/{name}");
    }
}
