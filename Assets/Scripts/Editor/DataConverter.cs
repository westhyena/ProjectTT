#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;


public class DataConverter : MonoBehaviour
{
    private static string NOTION_API_KEY = "secret_C3hToHmy8AH1RQvxN8E2DbWpB54jJcz2hfG74laOmz9";
    private static string NOTION_VERSION = "2022-06-28";

    private static string CHARACTER_INFO_DATABASE = "7e9bd778c8d340a485aa7a502eb28544";
    private static string WAVE_INFO_DATABASE = "ea199c438e5b44f8ba9826ee941c4aa7";
    private static string WAVE_GROUP_INFO_DATABASE = "9d942e801ef04543a5924b560ca28416";
    private static string STAGE_INFO_DATABASE = "c94148b20149490ab913824bf675c88a";

    [MenuItem("Data Manager/Download")]
    static void DownloadData()
    {
        Debug.Log("Download data");
        DownloadCharacterData();
        DownloadWaveData();
        DownloadWaveGroupData();
        DownloadStageData();
        AssetDatabase.Refresh();
        Debug.Log("Download data done!");
    }

    static void ConvertJArrayToCSV(JArray jArray, string outputPath)
    {
        if (jArray == null || jArray.Count == 0)
            return;

        StringBuilder csv = new ();
        // CSV 헤더 추가
        JObject firstObject = (JObject)jArray[0];
        IEnumerable<string> headers = firstObject.Properties().Select(prop => prop.Name);
        csv.AppendLine(string.Join(",", headers));

        // 각 JSON 객체를 CSV 행으로 변환
        foreach (JObject obj in jArray)
        {
            IEnumerable<string> values = obj.Properties().Select(prop => prop.Value.ToString());
            csv.AppendLine(string.Join(",", values));
        }

        // CSV 파일 저장
        File.WriteAllText(outputPath, csv.ToString());
    }

    static string GetString(JObject propertyObj, string key)
    {
        JObject obj = propertyObj.GetValue(key) as JObject;
        string typeStr = obj.GetValue("type").ToString();
        JArray typeAry = obj.GetValue(typeStr) as JArray;
        if (typeAry.Count == 0) return null;

        JObject typeObj = typeAry[0] as JObject;
        return typeObj.GetValue("plain_text").ToString();
    }

    static int GetInteger(JObject propertyObj, string key)
    {
        JObject obj = propertyObj.GetValue(key) as JObject;
        JToken intToken = obj.GetValue("number");
        if (intToken == null) return 0;

        int.TryParse(intToken.ToString(), out int intValue);
        return intValue;
    }

    static string GetRelationUUID(JObject propertyObj, string key)
    {
        JObject obj = propertyObj.GetValue(key) as JObject;
        JArray typeAry = obj.GetValue("relation") as JArray;
        if (typeAry.Count == 0) return null;

        JObject typeObj = typeAry[0] as JObject;
        return typeObj.GetValue("id").ToString();
    }

    static string GetRelationID(JObject propertyObj, string key, Dictionary<string, string> uuidDict)
    {
        string uuid = GetRelationUUID(propertyObj, key);
        if (uuid == null) return null;

        return uuidDict.GetValueOrDefault(uuid, null);
    }

    static JArray DownloadNotionDatabase(string databaseId)
    {
        string url = "https://api.notion.com/v1/databases/" + databaseId + "/query";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.method = "POST";
        request.SetRequestHeader("Authorization", "Bearer " + NOTION_API_KEY);
        request.SetRequestHeader("Notion-Version", NOTION_VERSION);
        var asyncOperation = request.SendWebRequest();

        while (!asyncOperation.isDone) {}

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        JObject data = JObject.Parse(request.downloadHandler.text);
        JArray results = data.GetValue("results") as JArray;
        return results;
    }

    static Dictionary<string, string> GetUUIDDictionary(string databaseId)
    {
        JArray results = DownloadNotionDatabase(databaseId);
        Dictionary<string, string> uuidDict = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            string uuid = obj.GetValue("id").ToString();
            uuidDict[uuid] = id;
        }
        return uuidDict;
    }

    static void DownloadCharacterData()
    {
        Debug.Log("DownloadCharacterData");
        JArray results = DownloadNotionDatabase(CHARACTER_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("name", GetString(propertyObj, "name"));
            newObj.Add("rangeOfTarget", GetInteger(propertyObj, "rangeOfTarget"));
            newObj.Add("baseMSpd", GetInteger(propertyObj, "baseMSpd"));
            newObj.Add("baseMaxHP", GetInteger(propertyObj, "baseMaxHP"));
            newObj.Add("baseAttack", GetInteger(propertyObj, "baseAttack"));
            newObj.Add("baseAtkSpd", GetInteger(propertyObj, "baseAtkSpd"));
            newObj.Add("baseDefense", GetInteger(propertyObj, "baseDefense"));
            newObj.Add("prefabKey", GetString(propertyObj, "prefabKey"));
            newObj.Add("iconSprite", GetString(propertyObj, "iconSprite"));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "CharacterData.csv");
        ConvertJArrayToCSV(ary, path);
    }

    static void DownloadWaveData()
    {
        Debug.Log("DownloadWaveData");
        JArray results = DownloadNotionDatabase(WAVE_INFO_DATABASE);
        Dictionary<string, string> characterDict = GetUUIDDictionary(CHARACTER_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("startTime", GetInteger(propertyObj, "startTime"));
            newObj.Add("monsterId", GetRelationID(propertyObj, "monsterId", characterDict));
            newObj.Add("totalCount", GetInteger(propertyObj, "totalCount"));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "WaveData.csv");
        ConvertJArrayToCSV(ary, path);
    }

    static void DownloadWaveGroupData()
    {
        Debug.Log("DownloadWaveGroupData");
        JArray results = DownloadNotionDatabase(WAVE_GROUP_INFO_DATABASE);
        Dictionary<string, string> waveDict = GetUUIDDictionary(WAVE_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();
            
            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("wave01", GetRelationID(propertyObj, "wave01", waveDict));
            newObj.Add("wave02", GetRelationID(propertyObj, "wave02", waveDict));
            newObj.Add("wave03", GetRelationID(propertyObj, "wave03", waveDict));
            newObj.Add("wave04", GetRelationID(propertyObj, "wave04", waveDict));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "WaveGroupData.csv");
        ConvertJArrayToCSV(ary, path);
    }

    static void DownloadStageData()
    {
        Debug.Log("DownloadStageData");
        JArray results = DownloadNotionDatabase(STAGE_INFO_DATABASE);
        Dictionary<string, string> waveGroupDict = GetUUIDDictionary(WAVE_GROUP_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();
            
            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("phase01waveGroup", GetRelationID(propertyObj, "phase01waveGroup", waveGroupDict));
            newObj.Add("phase02waveGroup", GetRelationID(propertyObj, "phase02waveGroup", waveGroupDict));
            newObj.Add("phase03waveGroup", GetRelationID(propertyObj, "phase03waveGroup", waveGroupDict));
            newObj.Add("phase04waveGroup", GetRelationID(propertyObj, "phase04waveGroup", waveGroupDict));
            newObj.Add("phase05waveGroup", GetRelationID(propertyObj, "phase05waveGroup", waveGroupDict));
            newObj.Add("stagePrefab", GetString(propertyObj, "stagePrefab"));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "StageData.csv");
        ConvertJArrayToCSV(ary, path);
    }
}
#endif
