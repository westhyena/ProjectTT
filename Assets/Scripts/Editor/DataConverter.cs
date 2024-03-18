#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Codice.CM.Common.Serialization;


public class DataConverter : MonoBehaviour
{
    private static string NOTION_API_KEY = "secret_C3hToHmy8AH1RQvxN8E2DbWpB54jJcz2hfG74laOmz9";
    private static string NOTION_VERSION = "2022-06-28";

    private static string CHARACTER_INFO_DATABASE = "7e9bd778c8d340a485aa7a502eb28544";

    [MenuItem("Data Manager/Download")]
    static void DownloadData()
    {
        Debug.Log("Download data");
        DownloadCharacterData();
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

    static void DownloadCharacterData()
    {
        JArray results = DownloadNotionDatabase(CHARACTER_INFO_DATABASE);
        JArray ary = new();
        foreach (JObject obj in results)
        {
            JObject newObj = new();

            JObject propertyObj = obj.GetValue("properties") as JObject;
            string id = GetString(propertyObj, "id");
            if (id == null) continue;

            newObj.Add("id", id);
            newObj.Add("name", GetString(propertyObj, "name"));
            newObj.Add("baseMSpd", GetInteger(propertyObj, "baseMSpd"));
            newObj.Add("baseMaxHP", GetInteger(propertyObj, "baseMaxHP"));
            newObj.Add("baseAttack", GetInteger(propertyObj, "baseAttack"));
            newObj.Add("baseAtkSpd", GetInteger(propertyObj, "baseAtkSpd"));
            newObj.Add("basePDef", GetInteger(propertyObj, "basePDef"));
            newObj.Add("prefabKey", GetString(propertyObj, "prefabKey"));
            newObj.Add("iconSprite", GetString(propertyObj, "iconSprite"));

            ary.Add(newObj);
        }
        string path = Path.Combine(Application.streamingAssetsPath, "CharacterData.csv");
        ConvertJArrayToCSV(ary, path);
    }
}
#endif
