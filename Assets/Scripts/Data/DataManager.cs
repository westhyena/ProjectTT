using System.Collections;
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

    readonly Dictionary<string, CharacterInfo> characterMap = new ();
    public CharacterInfo GetCharacterInfo(string id)
    {
        return characterMap.GetValueOrDefault(id, null);
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        ReadData();
    }

    void ReadData()
    {
        ReadCharacterData();
    }

    void ReadCharacterData()
    {
        characterMap.Clear();
        CharacterData loadedData = new CharacterData(ReadCSV("CharacterData.csv"));
        foreach (CharacterInfo item in loadedData.items)
        {
            if (item == null || string.IsNullOrEmpty(item.id))
                continue;

            characterMap.Add(item.id, item);
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
