using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; 
using System.Data; 
using System.Data.Odbc; 
using System.IO;
using System.Reflection;
using System.Timers;
using UnityEditor;

public class DataReader : EditorWindow {

  private enum EXls
  {
    TranslationData,
    Max
  }

  private static string lastMsg = string.Empty;
	static int END_OF_ROW= -1;
	static int END_OF_SHEET = -2;
	static int END_OF_COLUMNS_TYPE = -3;
	static int DUMMY_COLUMS_VAL = -4;

	private static int parseCount = 0;
	private static int parseAllCount = 20;
	private static float progress = 0f;

  private static GameObject DataInstance = null;
  private static DataMgr m_DataMgr = null;

	[MenuItem ("DataReaders/Data Reader")]
	public static void Init()
  {
		DataReader window = (DataReader)EditorWindow.GetWindow (typeof (DataReader));
	}

	private string openXlsPath;

	void OnGUI () 
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

		for (EXls Data = EXls.TranslationData ; Data < EXls.Max ; ++Data)
    {
      if(GUILayout.Button("Open "+ Data.ToString() + " Excel File", GUILayout.Height (30)))
      {
        openXlsPath = Path.GetFullPath("Assets/Editor/XlsData/"+ Data.ToString() + ".xls");
        EditorApplication.delayCall += StartOpenXls;
      }
    }

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.Space(10f);

		GUILayout.BeginHorizontal();
		if(GUILayout.Button("Import Data from Excel", GUILayout.Height (100)))
		{
			progress = 0f;
			parseCount = 0;
			parseAllCount = 1;
			EditorUtility.DisplayProgressBar("Parse...","Do parsing", progress);
			ReadExcelData();
		}
		GUILayout.EndHorizontal();
	}	

	void StartOpenXls()
	{
		System.Diagnostics.Process.Start(openXlsPath);
	}

	static string GetPath(string xlsPath)
	{
		xlsPath = Path.GetFullPath("Assets/Editor/XlsData/" + xlsPath);
		return xlsPath;
	}

	private static DataTable ReadSingleSheet(string sheetName, string excelFilePath)
	{
		DataTable retDataTable = null;
		retDataTable = new DataTable(sheetName);

		int retCnt = 0;
		int rowCnt = 0;
		int interateColumn = 0;
		try
		{
			var process = new System.Diagnostics.Process();
			string filePath = string.Empty;
			filePath = Application.dataPath.Replace("Assets", "bin/XlsParsingLinker.exe");
			var startInfo = new System.Diagnostics.ProcessStartInfo(filePath, "-path " + excelFilePath + " -s " + sheetName);
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.CreateNoWindow = true;
			startInfo.UseShellExecute = false;
			startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
			startInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
			process.StartInfo = startInfo;

			//Start!!
			process.Start();
			int mask = ((int)char.MaxValue / 2);
			int iter = 0;
			bool IsCompleteTypeIndexing = false;

			int columsLength = 0;

			DataRow newRow = null;
			while (!process.StandardOutput.EndOfStream) 
			{
				char[] sizeBuffer = new char[1];
				process.StandardOutput.ReadBlock(sizeBuffer, 0, 1);
				int blockSize = (int)sizeBuffer[0];
				blockSize -= mask;
				if(!IsCompleteTypeIndexing)
				{
					//first read colums data
					//if get input end of Colums type sign
					if(blockSize == END_OF_COLUMNS_TYPE)
					{
						IsCompleteTypeIndexing = true;
						newRow = retDataTable.NewRow();
						continue;
					}
					else if(blockSize < END_OF_COLUMNS_TYPE)
						continue;
					else
					{
						char[] buffer = new char[blockSize];
						process.StandardOutput.ReadBlock(buffer, 0, blockSize);
						DataColumn dc = new DataColumn(columsLength.ToString(), typeof(string));
						retDataTable.Columns.Add(dc);
						columsLength++;
					}
				} else
				{
					//make DataTable
					if(blockSize == END_OF_ROW)
					{
						if(interateColumn >= columsLength)
						{
							retDataTable.Rows.Add(newRow);
							newRow = retDataTable.NewRow();
							rowCnt++;
							interateColumn = 0;
						}
						else
						{
							for(int j = interateColumn; j < columsLength; j++)
							{
								newRow[j.ToString()] = "";
							}
							retDataTable.Rows.Add(newRow);
							newRow = retDataTable.NewRow();
							rowCnt++;
							interateColumn = 0;

						}
						//						Debug.Log("--------end of rows----------");
					} else if(blockSize == END_OF_SHEET)
					{
						break;
					}
					else
					{
						char[] buffer = new char[blockSize];
						process.StandardOutput.ReadBlock(buffer, 0, blockSize);
						string val = new string(buffer);

						if(interateColumn >= columsLength)
							continue;

						if(val.Contains("null"))
							val = "";
						newRow[interateColumn.ToString()] = val;
						interateColumn++;
					}
				}
			}
			retDataTable.AcceptChanges();
			bool errored = false;
			while (!process.StandardError.EndOfStream) 
			{
				errored = true;
				string line = process.StandardError.ReadLine();
				EditorUtility.DisplayDialog("Read Failed",line, "OK");
			}

			if(!errored)
				process.Close();
		}
		catch (Exception e)
		{
			string Msg = "[Dialogue]Sheet " + sheetName + " Checked Rows: " + (rowCnt+1).ToString() + "Cols: " + interateColumn + " \n" + e.Message;
			UnityEngine.Debug.LogError(Msg);
			throw e;
			return null;
		}
		finally {

		}
		return retDataTable;
	}

  static void SetProgress()
  {
      parseCount++;
      EditorUtility.DisplayProgressBar("Parse...","Do parsing", (float)parseCount/(float)parseAllCount);
  }

  static void ReadExcelData () {
      Debug.Log("RED S.T" + DateTime.UtcNow.ToString("0:yyyy/MM/dd HH:mm:ss.fff"));
      DateTime StartTime = DateTime.UtcNow; 

      if (Application.isPlaying)
      {
          lastMsg = "Cannot run Ally import when you playing. stop playing and try again";
          Debug.Log (lastMsg);
          return;
      }       

      DataInstance = new GameObject("DataMgr");

      try
      {
          if (m_DataMgr == null)
              m_DataMgr = DataInstance.AddComponent<DataMgr>();

          // 엑셀데이타 추출하는 부분.
		for (EXls i = EXls.TranslationData ; i != EXls.Max ; ++i)
          {
              if ( !SetXlsData(i) ) return;
          }

          SetProgress();

      } catch (Exception e)
      {
          UnityEngine.Debug.Log(e.Message);
          lastMsg = "Result : fail\nMessage : " + e.Message;
          if (DataInstance != null)
              GameObject.DestroyImmediate(DataInstance);
          EditorUtility.DisplayDialog("fail", lastMsg, "ok");
          return;
      } finally {
          EditorUtility.ClearProgressBar();

          if(DataInstance != null)
          {
                string prefabFilePath = "Assets/Resources/Prefabs/Mgr/DataMgr.prefab";
                GameObject oldPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabFilePath, typeof(GameObject));
                GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(m_DataMgr.gameObject, prefabFilePath);//.ReplacePrefab(DataInstance, oldPrefab, ReplacePrefabOptions.ReplaceNameBased);

                EditorUtility.SetDirty(DataInstance);
              GameObject.DestroyImmediate(DataInstance);
              EditorUtility.DisplayDialog("Success!!!", "DataMgr Write", "ok");
          }
      }
      DateTime EndTime = DateTime.UtcNow; 
      TimeSpan durationTime = StartTime - EndTime;
      Debug.Log("RED F.T" + DateTime.UtcNow.ToString("0:yyyy/MM/dd HH:mm:ss.fff"));
      Debug.Log("durationTime (totalSec) : " + durationTime.TotalSeconds);

  }

  static bool SetXlsData(EXls DataKind)
  {
    int breakSheet = 0;
    int breakRows = 0;
    List<String> sheetName = new List<string>();
    try
    {
      string DataPath = GetPath(DataKind.ToString() + ".xls");

      switch (DataKind)
      {
        case EXls.TranslationData:
          #region TranslationData
          string[] TranslationDataSheet = { "Translation", "ShipName", "SkillDesc", "TierName", "GachaName", "GachaDesc", "LoadingTip", "PushMSG" };

		  for (int i = 0; i < TranslationDataSheet.Length; ++i)
          {
            DataTable Table = ReadSingleSheet(TranslationDataSheet[i], DataPath);
            sheetName.Add(TranslationDataSheet[i]);

            if (Table.Rows.Count > 0)
            {
              for (int ii = 0; ii < Table.Rows.Count; ii++)
              {
                breakRows = ii;
                object[] obj = Table.Rows[ii].ItemArray;

                if (string.IsNullOrEmpty(Get<string>(obj, 0)))
                  continue;

                TranslationElement te = new TranslationElement();
                te.ID = Get<int>(obj, 0);
                te.Kor = Get<string>(obj, 1);
                te.Eng = Get<string>(obj, 2);
                te.Jpn = Get<string>(obj, 3);
                te.Prt = Get<string>(obj, 4);
                te.Deu = Get<string>(obj, 5);
                te.Rus = Get<string>(obj, 6);
                te.Fra = Get<string>(obj, 7);
                te.Chn_s = Get<string>(obj, 8);
                te.Chn_t = Get<string>(obj, 9);
                te.Esp = Get<string>(obj, 10);
                te.Tha = Get<string>(obj, 11);
                te.Vnm = Get<string>(obj, 12);
                te.Mys = Get<string>(obj, 13);
                
                m_DataMgr.m_TranslationElementDic.Add(te.ID, te);
              }
            }
            breakSheet++;
          }
          #endregion
          break;
      }
    }
    catch (Exception e)
    {
      UnityEngine.Debug.Log(e.Message);
      lastMsg = "Result : fail\nMessage : " + e.Message;
      lastMsg += "Break Sheet: " + sheetName[breakSheet] + ", Break Rows: " + breakRows;
      if (DataInstance != null)
        GameObject.DestroyImmediate(DataInstance);
      EditorUtility.DisplayDialog("fail", lastMsg, "ok");
      return false;
    }
    return true;
  }

  static T Get<T>(object[] value,int index)
  {
    if (string.IsNullOrEmpty((string)Convert.ChangeType(value[index], typeof(string))))
      return default(T);
    return (T)Convert.ChangeType(value[index],typeof(T));
  }
}
