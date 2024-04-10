#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class DataValidate : DataManager
{
    [MenuItem("Data Manager/Validate")]
    static void ValidateDate()
    {
        Debug.Log("Validate Data");
        DataValidate instance = new ();
        instance.Validate();
        Debug.Log("Validate Success");
    }

    public void Validate()
    {
        this.ReadData();
        foreach (CharacterInfo info in this.characterMap.Values)
        {
            if (!string.IsNullOrEmpty(info.prefabKey))
            {
                if (ResourceManager.GetCharacterPrefab(info.prefabKey) == null)
                {
                    Debug.LogWarning($"Not found character prefab: {info.prefabKey}");
                }
            }
            if (!string.IsNullOrEmpty(info.iconSprite))
            {
                if (ResourceManager.GetCharacterIcon(info.iconSprite) == null)
                {
                    Debug.LogWarning($"Not found character icon: {info.iconSprite}");
                }
            }
        }
        
        foreach (SkillInfo info in this.skillMap.Values)
        {
            if (!string.IsNullOrEmpty(info.atkAnimation))
            {
                if (ResourceManager.GetSkillPrefab(info.atkAnimation) == null)
                {
                    Debug.LogWarning($"Not found skill prefab: {info.atkAnimation}");
                }
            }
            if (!string.IsNullOrEmpty(info.atkedVFX))
            {
                if (ResourceManager.GetHitPrefab(info.atkedVFX) == null)
                {
                    Debug.LogWarning($"Not found hit prefab: {info.atkedVFX}");
                }
            }
        }

        foreach (ProjectileInfo info in this.projectileMap.Values)
        {
            if (!string.IsNullOrEmpty(info.projectilePrefab))
            {
                if (ResourceManager.GetProjectilePrefab(info.projectilePrefab) == null)
                {
                    Debug.LogWarning($"Not found projectile prefab: {info.projectilePrefab}");
                }
            }
        }

        foreach (StageInfo info in this.stageMap.Values)
        {
            if (!string.IsNullOrEmpty(info.stagePrefab))
            {
                if (ResourceManager.GetStagePrefab(info.stagePrefab) == null)
                {
                    Debug.LogWarning($"Not found stage prefab: {info.stagePrefab}");
                }
            }

            if (!string.IsNullOrEmpty(info.phase01waveGroup))
            {
                if (this.GetWaveGroupInfo(info.phase01waveGroup) == null)
                {
                    Debug.LogWarning($"Not found wave group: {info.phase01waveGroup}");
                }
            }
            if (!string.IsNullOrEmpty(info.phase02waveGroup))
            {
                if (this.GetWaveGroupInfo(info.phase02waveGroup) == null)
                {
                    Debug.LogWarning($"Not found wave group: {info.phase02waveGroup}");
                }
            }
            if (!string.IsNullOrEmpty(info.phase03waveGroup))
            {
                if (this.GetWaveGroupInfo(info.phase03waveGroup) == null)
                {
                    Debug.LogWarning($"Not found wave group: {info.phase03waveGroup}");
                }
            }
            if (!string.IsNullOrEmpty(info.phase04waveGroup))
            {
                if (this.GetWaveGroupInfo(info.phase04waveGroup) == null)
                {
                    Debug.LogWarning($"Not found wave group: {info.phase04waveGroup}");
                }
            }
            if (!string.IsNullOrEmpty(info.phase05waveGroup))
            {
                if (this.GetWaveGroupInfo(info.phase05waveGroup) == null)
                {
                    Debug.LogWarning($"Not found wave group: {info.phase05waveGroup}");
                }
            }
        }

        foreach (WaveGroupInfo info in this.waveGroupMap.Values)
        {
            if (!string.IsNullOrEmpty(info.wave01))
            {
                if (this.GetWaveInfo(info.wave01) == null)
                {
                    Debug.LogWarning($"Not found wave: {info.wave01}");
                }
            }
            if (!string.IsNullOrEmpty(info.wave02))
            {
                if (this.GetWaveInfo(info.wave02) == null)
                {
                    Debug.LogWarning($"Not found wave: {info.wave02}");
                }
            }
            if (!string.IsNullOrEmpty(info.wave03))
            {
                if (this.GetWaveInfo(info.wave03) == null)
                {
                    Debug.LogWarning($"Not found wave: {info.wave03}");
                }
            }
            if (!string.IsNullOrEmpty(info.wave04))
            {
                if (this.GetWaveInfo(info.wave04) == null)
                {
                    Debug.LogWarning($"Not found wave: {info.wave04}");
                }
            }
        }

        foreach (WaveInfo wave in this.waveMap.Values)
        {
            if (!string.IsNullOrEmpty(wave.monsterId))
            {
                if (this.GetCharacterInfo(wave.monsterId) == null)
                {
                    Debug.LogWarning($"Not found character: {wave.monsterId}");
                }
            }
        }
    }
}
#endif
