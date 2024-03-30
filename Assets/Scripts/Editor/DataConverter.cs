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
    private static readonly string NOTION_API_KEY = "secret_C3hToHmy8AH1RQvxN8E2DbWpB54jJcz2hfG74laOmz9";
    private static readonly string NOTION_VERSION = "2022-06-28";

    private static readonly string CONST_DATABASE = "8a7b2a32f5744082b60d4f960ffab47a";

    private static readonly string CHARACTER_INFO_DATABASE = "7e9bd778c8d340a485aa7a502eb28544";
    private static readonly string ATTACK_TYPE_DATABASE = "c1251a8f05a74a9db70534afe25663bd";
    private static readonly string ATTACK_ATTRIBUTE_DATEBASE = "cdebc8bb07e04a67954695971beef66c";
    private static readonly string CHATACTER_LEVEL_INFO_DATABASE = "2ac8a738d91046c1898d54b238305886";

    private static readonly string WAVE_INFO_DATABASE = "ea199c438e5b44f8ba9826ee941c4aa7";
    private static readonly string WAVE_GROUP_INFO_DATABASE = "9d942e801ef04543a5924b560ca28416";
    private static readonly string STAGE_INFO_DATABASE = "c94148b20149490ab913824bf675c88a";

    [MenuItem("Data Manager/Download")]
    static void DownloadData()
    {
        Debug.Log("Download data");
        DownloadConstData();
        DownloadCharacterData();
        DownloadAttackTypeData();
        DownloadCharacterLevelData();
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

    static bool GetCheckbox(JObject propertyObj, string key)
    {
        JObject obj = propertyObj.GetValue(key) as JObject;
        JToken checkboxToken = obj.GetValue("checkbox");
        if (checkboxToken == null) return false;
        return obj.GetValue("checkbox").ToString() == "true";
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
        return GetUUIDDictionary(databaseId, "id");
    }

    static void SaveToCsv(JArray ary, string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        ConvertJArrayToCSV(ary, path);
    }

    static Dictionary<string, string> GetUUIDDictionary(string databaseId, string idPropertyKey)
    {
        JArray results = DownloadNotionDatabase(databaseId);
        Dictionary<string, string> uuidDict = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, idPropertyKey);
            if (id == null) continue;
            string uuid = obj.GetValue("id").ToString();
            uuidDict[uuid] = id;
        }
        return uuidDict;
    }

    static void DownloadConstData()
    {
        Debug.Log("DownloadConstData");
        JArray results = DownloadNotionDatabase(CONST_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string constName = GetString(propertyObj, "constName");
            if (constName == null) continue;
            
            newObj.Add("constName", constName);
            newObj.Add("value", GetString(propertyObj, "value"));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "ConstData.csv");
        ConvertJArrayToCSV(ary, path);
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
        SaveToCsv(ary, "CharacterData.csv");
    }

    static void DownloadAttackTypeData()
    {
        Debug.Log("DownloadAttackTypeData");
        JArray results = DownloadNotionDatabase(ATTACK_TYPE_DATABASE);
        Dictionary<string, string> attributeDict = GetUUIDDictionary(ATTACK_ATTRIBUTE_DATEBASE, "index");

        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("memo", GetString(propertyObj, "memo"));
            newObj.Add("atkAttribute", GetRelationID(propertyObj, "atkAttribute", attributeDict));
            newObj.Add("isTargetToGround", GetCheckbox(propertyObj, "isTargetToGround"));
            newObj.Add("isTargetToAir", GetCheckbox(propertyObj, "isTargetToAir"));
            newObj.Add("isRangeAttack", GetCheckbox(propertyObj, "isRangeAttack"));
            newObj.Add("atkWeight", GetInteger(propertyObj, "atkWeight"));
            newObj.Add("atkSpdWeight", GetInteger(propertyObj, "atkSpdWeight"));
            newObj.Add("hpWeight", GetInteger(propertyObj, "hpWeight"));
            newObj.Add("defWeight", GetInteger(propertyObj, "defWeight"));

            ary.Add(newObj);
        }
        SaveToCsv(ary, "AttackTypeData.csv");
    }

    static void DownloadCharacterLevelData()
    {
        Debug.Log("DownloadCharacterLevelData");
        JArray results = DownloadNotionDatabase(CHATACTER_LEVEL_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results.Cast<JObject>())
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("levelType", GetInteger(propertyObj, "levelType"));
            newObj.Add("level", GetInteger(propertyObj, "level"));
            newObj.Add("requiredExp", GetInteger(propertyObj, "requiredExp"));
            newObj.Add("atkMultiple", GetInteger(propertyObj, "atkMultiple"));
            newObj.Add("atkSpdMultiple", GetInteger(propertyObj, "atkSpdMultiple"));
            newObj.Add("defMultiple", GetInteger(propertyObj, "defMultiple"));
            newObj.Add("maxHPMultiple", GetInteger(propertyObj, "maxHPMultiple"));

            ary.Add(newObj);
        }
        SaveToCsv(ary, "CharacterLevelData.csv");
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
        SaveToCsv(ary, "WaveData.csv");
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
        SaveToCsv(ary, "WaveGroupData.csv");
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
        SaveToCsv(ary, "StageData.csv");
    }
}
#endif
