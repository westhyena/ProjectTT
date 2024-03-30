using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<DataManager>();
            }
            if (null == _instance)
            {
                GameObject gameObject = new("DataManager");
                _instance = gameObject.AddComponent<DataManager>();
            }
            return _instance;
        }
    }

    public class DataMap<T>: Dictionary<string, T> {}

    readonly DataMap<string> constMap = new ();
    public string GetConstValue(string constName)
    {
        return constMap.GetValueOrDefault(constName, null);
    }

    readonly DataMap<AttackTypeInfo> attackTypeMap = new ();
    public AttackTypeInfo GetAttackTypeInfo(string id)
    {
        return attackTypeMap.GetValueOrDefault(id, null);
    }

    readonly DataMap<CharacterInfo> characterMap = new ();
    public CharacterInfo GetCharacterInfo(string id)
    {
        return characterMap.GetValueOrDefault(id, null);
    }

    readonly List<CharacterLevelInfo> outgameLevelList = new ();
    readonly List<CharacterLevelInfo> playerLevelList = new ();
    readonly List<CharacterLevelInfo> companionLevelList = new ();

    readonly DataMap<WaveInfo> waveMap = new ();
    public WaveInfo GetWaveInfo(string id)
    {
        if (id == null) return null;
        return waveMap.GetValueOrDefault(id, null);
    }

    readonly DataMap<WaveGroupInfo> waveGroupMap = new ();
    public WaveGroupInfo GetWaveGroupInfo(string id)
    {
        if (id == null) return null;
        return waveGroupMap.GetValueOrDefault(id, null);
    }

    readonly DataMap<StageInfo> stageMap = new ();
    public StageInfo GetStageInfo(string id)
    {
        return stageMap.GetValueOrDefault(id, null);
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        ReadData();
    }

    void ReadData()
    {
        ReadConstData();
        ReadCharacterData();
        ReadAttackTypeData();
        ReadCharacterLevelData();
        ReadWaveData();
        ReadWaveGroupData();
        ReadStageData();
    }

    void ReadConstData()
    {
        constMap.Clear();
        ConstData loadedData = new(ReadCSV("ConstData.csv"));
        foreach (ConstInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.constName))
                continue;

            constMap.Add(item.constName, item.value);
        }
    }

    void ReadCharacterData()
    {
        characterMap.Clear();
        CharacterData loadedData = new(ReadCSV("CharacterData.csv"));
        foreach (CharacterInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            characterMap.Add(item.id, item);
        }
    }

    void ReadAttackTypeData()
    {
        attackTypeMap.Clear();
        AttackTypeData loadedData = new(ReadCSV("AttackTypeData.csv"));
        foreach (AttackTypeInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            attackTypeMap.Add(item.id, item);
        }
    }

    void ReadCharacterLevelData()
    {
        outgameLevelList.Clear();
        playerLevelList.Clear();
        companionLevelList.Clear();
        CharacterLevelData loadedData = new(ReadCSV("CharacterLevelData.csv"));
        foreach (CharacterLevelInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            if (item.levelType == 0)
            {
                outgameLevelList.Add(item);
            }
            else if (item.levelType == 1)
            {
                playerLevelList.Add(item);
            }
            else if (item.levelType == 2)
            {
                companionLevelList.Add(item);
            }
        }

        outgameLevelList.Sort((a, b) => a.level - b.level);
        playerLevelList.Sort((a, b) => a.level - b.level);
        companionLevelList.Sort((a, b) => a.level - b.level);
    }

    void ReadWaveData()
    {
        waveMap.Clear();
        WaveData loadedData = new(ReadCSV("WaveData.csv"));
        foreach (WaveInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            waveMap.Add(item.id, item);
        }
    }

    void ReadWaveGroupData()
    {
        waveGroupMap.Clear();
        WaveGroupData loadedData = new(ReadCSV("WaveGroupData.csv"));
        foreach (WaveGroupInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            waveGroupMap.Add(item.id, item);
        }
    }

    void ReadStageData()
    {
        stageMap.Clear();
        StageData loadedData = new(ReadCSV("StageData.csv"));
        foreach (StageInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            stageMap.Add(item.id, item);
        }
    }

    public static string[,] ReadCSV(string fileName)
    {
        string pathRoot = Application.streamingAssetsPath;
        string filePath = Path.Combine(pathRoot, fileName);
#if !UNITY_EDITOR && UNITY_ANDROID
        WWW reader = new WWW(filePath);
        while (!reader.isDone) {}
        string dataAsJson = reader.text;
#else
        string dataAsJson = File.ReadAllText(filePath);
#endif
        return CSVReader.SplitCsvGrid(dataAsJson);
    }
}
