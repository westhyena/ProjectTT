using System.Collections.Generic;
using System.IO;
using UnityEngine;

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

    readonly DataMap<CharacterInfo> characterMap = new ();
    public CharacterInfo GetCharacterInfo(string id)
    {
        return characterMap.GetValueOrDefault(id, null);
    }

    readonly DataMap<WaveInfo> waveMap = new ();
    public WaveInfo GetWaveInfo(string id)
    {
        return waveMap.GetValueOrDefault(id, null);
    }

    readonly DataMap<WaveGroupInfo> waveGroupMap = new ();
    public WaveGroupInfo GetWaveGroupInfo(string id)
    {
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
        ReadCharacterData();
        ReadWaveData();
        ReadWaveGroupData();
        ReadStageData();
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
