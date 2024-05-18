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

public class DataReader : EditorWindow
{

	private enum EXls
	{
		TranslationData,
		InGameSystemData,
		StageWaveData,
		UserActiveSkillData,
		SkillData,
		CharacterData,
		Ingame_CharacterGrowData,
		UserSelectCard,
		UserSelectCardRandomTable,
		Max
	}

	private static string lastMsg = string.Empty;
	static int END_OF_ROW = -1;
	static int END_OF_SHEET = -2;
	static int END_OF_COLUMNS_TYPE = -3;
	static int DUMMY_COLUMS_VAL = -4;

	private static int parseCount = 0;
	private static int parseAllCount = 20;
	private static float progress = 0f;

	private static GameObject DataInstance = null;
	private static DataMgr m_DataMgr = null;

	[MenuItem("DataReaders/Data Reader")]
	public static void Init()
	{
		DataReader window = (DataReader)EditorWindow.GetWindow(typeof(DataReader));
	}

	private string openXlsPath;

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

		for (EXls Data = EXls.TranslationData; Data < EXls.Max; ++Data)
		{
			if (GUILayout.Button("Open " + Data.ToString() + " Excel File", GUILayout.Height(30)))
			{
				openXlsPath = Path.GetFullPath("Assets/Editor/XlsData/" + Data.ToString() + ".xls");
				EditorApplication.delayCall += StartOpenXls;
			}
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.Space(10f);

		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Import Data from Excel", GUILayout.Height(100)))
		{
			progress = 0f;
			parseCount = 0;
			parseAllCount = 1;
			EditorUtility.DisplayProgressBar("Parse...", "Do parsing", progress);
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
				if (!IsCompleteTypeIndexing)
				{
					//first read colums data
					//if get input end of Colums type sign
					if (blockSize == END_OF_COLUMNS_TYPE)
					{
						IsCompleteTypeIndexing = true;
						newRow = retDataTable.NewRow();
						continue;
					}
					else if (blockSize < END_OF_COLUMNS_TYPE)
						continue;
					else
					{
						char[] buffer = new char[blockSize];
						process.StandardOutput.ReadBlock(buffer, 0, blockSize);
						DataColumn dc = new DataColumn(columsLength.ToString(), typeof(string));
						retDataTable.Columns.Add(dc);
						columsLength++;
					}
				}
				else
				{
					//make DataTable
					if (blockSize == END_OF_ROW)
					{
						if (interateColumn >= columsLength)
						{
							retDataTable.Rows.Add(newRow);
							newRow = retDataTable.NewRow();
							rowCnt++;
							interateColumn = 0;
						}
						else
						{
							for (int j = interateColumn; j < columsLength; j++)
							{
								newRow[j.ToString()] = "";
							}
							retDataTable.Rows.Add(newRow);
							newRow = retDataTable.NewRow();
							rowCnt++;
							interateColumn = 0;

						}
						//						Debug.Log("--------end of rows----------");
					}
					else if (blockSize == END_OF_SHEET)
					{
						break;
					}
					else
					{
						char[] buffer = new char[blockSize];
						process.StandardOutput.ReadBlock(buffer, 0, blockSize);
						string val = new string(buffer);

						if (interateColumn >= columsLength)
							continue;

						if (val.Contains("null"))
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
				EditorUtility.DisplayDialog("Read Failed", line, "OK");
			}

			if (!errored)
				process.Close();
		}
		catch (Exception e)
		{
			string Msg = "[Dialogue]Sheet " + sheetName + " Checked Rows: " + (rowCnt + 1).ToString() + "Cols: " + interateColumn + " \n" + e.Message;
			UnityEngine.Debug.LogError(Msg);
			throw e;
			return null;
		}
		finally
		{

		}
		return retDataTable;
	}

	static void SetProgress()
	{
		parseCount++;
		EditorUtility.DisplayProgressBar("Parse...", "Do parsing", (float)parseCount / (float)parseAllCount);
	}

	static void ReadExcelData()
	{
		Debug.Log("RED S.T" + DateTime.UtcNow.ToString("0:yyyy/MM/dd HH:mm:ss.fff"));
		DateTime StartTime = DateTime.UtcNow;

		if (Application.isPlaying)
		{
			lastMsg = "Cannot run Ally import when you playing. stop playing and try again";
			Debug.Log(lastMsg);
			return;
		}

		DataInstance = new GameObject("DataMgr");

		try
		{
			if (m_DataMgr == null)
				m_DataMgr = DataInstance.AddComponent<DataMgr>();

			// 엑셀데이타 추출하는 부분.
			for (EXls i = EXls.TranslationData; i != EXls.Max; ++i)
			{
				if (!SetXlsData(i)) return;
			}

			SetProgress();

		}
		catch (Exception e)
		{
			UnityEngine.Debug.Log(e.Message);
			lastMsg = "Result : fail\nMessage : " + e.Message;
			if (DataInstance != null)
				GameObject.DestroyImmediate(DataInstance);
			EditorUtility.DisplayDialog("fail", lastMsg, "ok");
			return;
		}
		finally
		{
			EditorUtility.ClearProgressBar();

			if (DataInstance != null)
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
					string[] TranslationDataSheet = { "Text", "HeroName", "HeroDesc", "MercenaryName", "MercenaryDesc", "MonsterName", "MonsterDesc", "MiddleBossName", "MiddleBossDesc", "BossName", "BossDesc", 
						"UserActiveSkill","UserActiveSkillDesc","Stage","StageDesc","UserSelectCard","UserSelectCardDesc","SkillName", "SkillDesc" };

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
								te.ID = Get<string>(obj, 0);
								te.Kor = Get<string>(obj, 1);
								te.Eng = Get<string>(obj, 2);
								//te.Jpn = Get<string>(obj, 3);
								//te.Prt = Get<string>(obj, 4);
								//te.Deu = Get<string>(obj, 5);
								//te.Rus = Get<string>(obj, 6);
								//te.Fra = Get<string>(obj, 7);
								//te.Chn_s = Get<string>(obj, 8);
								//te.Chn_t = Get<string>(obj, 9);
								//te.Esp = Get<string>(obj, 10);
								//te.Tha = Get<string>(obj, 11);
								//te.Vnm = Get<string>(obj, 12);
								//te.Mys = Get<string>(obj, 13);

								m_DataMgr.m_TranslationElementDic.Add(te.ID, te);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.InGameSystemData:
					#region InGameSystemData
					string[] InGameSystemDataSheet = { "MercenaryPoint", "MercenaryLevelUpPoint", "SummonPoint", "CallPoint" };

					InGameSystemElement igse = new InGameSystemElement();
					for (int i = 0; i < InGameSystemDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(InGameSystemDataSheet[i], DataPath);
						sheetName.Add(InGameSystemDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;
								switch (InGameSystemDataSheet[i])
								{
									case "MercenaryPoint":
										igse.MercenaryPointGetTime = Get<int>(obj, 0);
										igse.GetMercenaryPoint = Get<int>(obj, 1);
										break;
									case "MercenaryLevelUpPoint":
										igse.Mercenary_LevelUp_NeedPoint.Add(Get<int>(obj, 1));
										break;
									case "SummonPoint":
										igse.Summon_NeedPoint = Get<int>(obj, 0);
										break;
									case "CallPoint":
										igse.Call_NeedPoint = Get<int>(obj, 0);
										break;
								}
								m_DataMgr.m_InGameSystemElement = igse;
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.StageWaveData:
					#region StageWaveData
					string[] StageWaveDataSheet = { "StageData" };


					for (int i = 0; i < StageWaveDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(StageWaveDataSheet[i], DataPath);
						sheetName.Add(StageWaveDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								StageWaveDataElement swde = new StageWaveDataElement();
								swde.id = Get<int>(obj, 0);
								swde.StageName = Get<string>(obj, 1);
								swde.StageNameID = Get<string>(obj, 2);
								swde.StageDescID = Get<string>(obj, 3);
								swde.PlayTime = Get<float>(obj, 4);
								swde.StartMercenaryPoint = Get<int>(obj, 5);
								int StageID = Get<int>(obj, 5);

								DataTable WaveTable = ReadSingleSheet(string.Format("Stage_{0}", StageID), DataPath);
								//웨이브 셋팅
								if (WaveTable.Rows.Count > 0)
								{
									for (int iii = 0; iii < WaveTable.Rows.Count; iii++)
									{
										breakRows = iii;
										object[] obj2 = WaveTable.Rows[iii].ItemArray;

										if (string.IsNullOrEmpty(Get<string>(obj2, 0)))
											continue;

										WaveDataInfo info = new WaveDataInfo();
										info.WaveID = Get<int>(obj2, 0);
										info.SummonTime = Get<float>(obj2, 1);
										info.ObjName = Get<string>(obj2, 2);
										info.CharacterID = Get<int>(obj2, 3);
										info.MonsterType = (MonsterType_E)Get<int>(obj2, 4);
										info.CharacterLevel = Get<int>(obj2, 5);
										info.SummonCount = Get<int>(obj2, 6);

										switch (info.WaveID)
										{
											case 0:
												swde.Wave0.Add(info);
												break;
											case 1:
												swde.Wave1.Add(info);
												break;
											case 2:
												swde.Wave2.Add(info);
												break;
											case 3:
												swde.Wave3.Add(info);
												break;
											case 4:
												swde.Wave4.Add(info);
												break;
										}
									}
								}

								DataTable FileTable = ReadSingleSheet("StageData_File", DataPath);
								object[] File = FileTable.Rows[swde.id].ItemArray;

								swde.IconName = Get<string>(File, 2);
								swde.StageMapObjectName = Get<string>(File, 3);

								m_DataMgr.m_StageWaveDataElementDic.Add(swde.id, swde);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.UserActiveSkillData:
					#region UserActiveSkillData
					string[] UserActiveSkillDataSheet = { "UserSkill" };

					for (int i = 0; i < UserActiveSkillDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(UserActiveSkillDataSheet[i], DataPath);
						sheetName.Add(UserActiveSkillDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								UserActiveSkillDataElement uasde = new UserActiveSkillDataElement();
								uasde.ID = Get<int>(obj, 0);
								uasde.UserSkillName = Get<string>(obj, 1);
								uasde.UserSkillNameID = Get<string>(obj, 2);
								uasde.UserSkillDescID = Get<string>(obj, 3);
								uasde.Rating = (Rating_E)Get<int>(obj, 4);
								uasde.PriceType = (Goods_E)Get<int>(obj, 5);
								uasde.PriceValue = Get<int>(obj, 6);
								uasde.ActivePosition = (ActivePosition_E)Get<int>(obj, 7);
								uasde.Target = (Target_E)Get<int>(obj, 8);
								if (string.IsNullOrEmpty(Get<string>(obj, 9)) == false)
									uasde.CoolTime = Get<float>(obj, 9);
								uasde.Type = (DamageType_E)Get<int>(obj, 10);
								uasde.TargetSelectType = (TargetSelect_E)Get<int>(obj, 11);
								if (string.IsNullOrEmpty(Get<string>(obj, 12)) == false)
									uasde.DamageTypeRange = Get<int>(obj, 12);

								int _Value = 0;
								float _Time = 0f;
								for (int sk = 13; sk <= 24; ++sk)
								{
									switch (sk)
									{
										case 13:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 13)) ? 0 : Get<int>(obj, 13);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Damage, _Value));
											break;
										case 14:
										case 15:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 14)) ? 0 : Get<int>(obj, 14);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 15)) ? 0 : Get<float>(obj, 15);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.DotDamage, _Value, _Time));
											++sk;
											break;
										case 16:
										case 17:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 16)) ? 0 : Get<int>(obj, 16);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 17)) ? 0 : Get<float>(obj, 17);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackUp, _Value, _Time));
											++sk;
											break;
										case 18:
										case 19:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 18)) ? 0 : Get<int>(obj, 18);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 19)) ? 0 : Get<float>(obj, 19);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackSpeedUp, 0, _Time));
											++sk;
											break;
										case 20:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 20)) ? 0 : Get<int>(obj, 20);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Heal, _Value));
											break;
										case 21:
											_Time = string.IsNullOrEmpty(Get<string>(obj, 21)) ? 0 : Get<float>(obj, 21);
											if (_Time != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Immunity, 0, _Time));
											break;
										case 22:
											_Time = string.IsNullOrEmpty(Get<string>(obj, 22)) ? 0 : Get<float>(obj, 22);
											if (_Time != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Stun, 0, _Time));
											break;
										case 23:
										case 24:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 23)) ? 0 : Get<int>(obj, 23);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 24)) ? 0 : Get<float>(obj, 24);
											if (_Value != 0)
												uasde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackSpeedDown, _Value, _Time));
											++sk;
											break;
									}
								}

								DataTable UserSkill_FileTable = ReadSingleSheet("UserSkill_File", DataPath);
								//파일명 셋팅
								object[] obj2 = UserSkill_FileTable.Rows[uasde.ID].ItemArray;

								//if (string.IsNullOrEmpty(Get<string>(obj2, 0)))
								//	continue;
								uasde.IconName = Get<string>(obj2, 2);
								uasde.Eff_SkillName = Get<string>(obj2, 3);

								m_DataMgr.m_UserActiveSkillDataElementDic.Add(uasde.ID, uasde);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.SkillData:
					#region SkillData
					string[] SkillDataSheet = { "Skill_0", "Skill_1", "Skill_2", "Skill_3", "Skill_4", "Skill_5" };

					for (int i = 0; i < SkillDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(SkillDataSheet[i], DataPath);
						sheetName.Add(SkillDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								SkillDataElement sde = new SkillDataElement();
								sde.ID = Get<int>(obj, 0);
								sde.UserSkillName = Get<string>(obj, 1);
								sde.UserSkillNameID = Get<string>(obj, 2);
								sde.UserSkillDescID = Get<string>(obj, 3);
								if (string.IsNullOrEmpty(Get<string>(obj, 4)))
									sde.DoAerialUnitAttack = Get<int>(obj, 4) == 0 ? false : true;
								if (string.IsNullOrEmpty(Get<string>(obj, 5)) == false)
									sde.ActivePosition = (ActivePosition_E)Get<int>(obj, 5);
								if (string.IsNullOrEmpty(Get<string>(obj, 6)) == false)
									sde.Target = (Target_E)Get<int>(obj, 6);
								if (string.IsNullOrEmpty(Get<string>(obj, 7)) == false)
									sde.CoolTime = Get<float>(obj, 7);
								if (string.IsNullOrEmpty(Get<string>(obj, 8)) == false)
									sde.Type = (DamageType_E)Get<int>(obj, 8);
								if (string.IsNullOrEmpty(Get<string>(obj, 9)) == false)
									sde.TargetSelectType = (TargetSelect_E)Get<int>(obj, 9);
								if (string.IsNullOrEmpty(Get<string>(obj, 10)) == false)
									sde.DamageTypeRange = Get<float>(obj, 10);

								float _Value = 0f;
								float _Time = 0f;
								for (int sk = 11; sk <= 22; ++sk)
								{
									switch (sk)
									{
										case 11:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 10)) ? 0 : Get<float>(obj, 11);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Damage, _Value));
											break;
										case 12://도트뎀
										case 13:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 12)) ? 0 : Get<float>(obj, 12);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 13)) ? 0 : Get<float>(obj, 13);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.DotDamage, _Value, _Time));
											++sk;
											break;
										case 14://공격력 증가
										case 15:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 14)) ? 0 : Get<float>(obj, 14);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 15)) ? 0 : Get<float>(obj, 15);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackUp, _Value, _Time));
											++sk;
											break;
										case 16://공속 증가
										case 17:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 16)) ? 0 : Get<float>(obj, 16);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 17)) ? 0 : Get<float>(obj, 17);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackSpeedUp, 0, _Time));
											++sk;
											break;
										case 18://물방 증가
										case 19:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 18)) ? 0 : Get<float>(obj, 18);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 19)) ? 0 : Get<float>(obj, 19);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.PDUp, 0, _Time));
											++sk;
											break;
										case 20://마방 증가
										case 21:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 20)) ? 0 : Get<float>(obj, 20);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 21)) ? 0 : Get<float>(obj, 21);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.MDUp, 0, _Time));
											++sk;
											break;
										case 22://크리티컬 확률 증가
										case 23:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 20)) ? 0 : Get<float>(obj, 20);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 21)) ? 0 : Get<float>(obj, 21);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.CriticalPerUp, 0, _Time));
											++sk;
											break;
										case 24://크리티컬 데미지 증가
										case 25:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 20)) ? 0 : Get<float>(obj, 20);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 21)) ? 0 : Get<float>(obj, 21);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.CriticalDamageUp, 0, _Time));
											++sk;
											break;
										case 26://힐
											_Value = string.IsNullOrEmpty(Get<string>(obj, 26)) ? 0 : Get<float>(obj, 26);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Heal, _Value));
											break;
										case 27://무적
											_Time = string.IsNullOrEmpty(Get<string>(obj, 27)) ? 0 : Get<float>(obj, 27);
											if (_Time != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Immunity, 0, _Time));
											break;
										case 28://기절시간
											_Time = string.IsNullOrEmpty(Get<string>(obj, 28)) ? 0 : Get<float>(obj, 28);
											if (_Time != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.Stun, 0, _Time));
											break;
										case 29://공속 감소
										case 30:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 29)) ? 0 : Get<float>(obj, 29);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 30)) ? 0 : Get<float>(obj, 30);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackSpeedDown, _Value, _Time));
											++sk;
											break;
										case 31://공격력 감소
										case 32:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 31)) ? 0 : Get<float>(obj, 31);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 32)) ? 0 : Get<float>(obj, 32);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.AttackDown, _Value, _Time));
											++sk;
											break;
										case 33://이동속도 감소
										case 34:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 33)) ? 0 : Get<float>(obj, 33);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 34)) ? 0 : Get<float>(obj, 34);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.MoveSpeedDown, _Value, _Time));
											++sk;
											break;
										case 35://물방 감소
										case 36:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 35)) ? 0 : Get<float>(obj, 35);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 36)) ? 0 : Get<float>(obj, 36);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.PDDown, _Value, _Time));
											++sk;
											break;
										case 37://마방 감소
										case 38:
											_Value = string.IsNullOrEmpty(Get<string>(obj, 37)) ? 0 : Get<float>(obj, 37);
											_Time = string.IsNullOrEmpty(Get<string>(obj, 38)) ? 0 : Get<float>(obj, 38);
											if (_Value != 0)
												sde.SkillData.Add(new SkillDataBase((int)SkillDataKind_E.MDDown, _Value, _Time));
											++sk;
											break;
									}
								}

								DataTable UserSkill_FileTable = ReadSingleSheet(string.Format("Skill_{0}File", i), DataPath);
								//파일명 셋팅
								object[] obj2 = UserSkill_FileTable.Rows[ii].ItemArray;

								//if (string.IsNullOrEmpty(Get<string>(obj2, 0)))
								//	continue;
								sde.IconName = Get<string>(obj2, 2);
								sde.ProjectileEffectName = Get<string>(obj2, 3);
								sde.ObjectDamageEffectName = Get<string>(obj2, 4);
								sde.MyPositionEffectName = Get<string>(obj2, 5);
								sde.TargetPointEffectName = Get<string>(obj2, 6);

								sde.BuffIcons = new List<BuffIcons>();
								sde.deBuffIcons = new List<BuffIcons>();

								if ( string.IsNullOrEmpty( Get<string>(obj2,7)) == false )
								{
									string[] Buff = Get<string>(obj2, 7).Split(',');
									for(int b = 0; b < Buff.Length; ++b)
									{
										BuffIcons bi = new BuffIcons(Buff[b]);
										sde.BuffIcons.Add(bi);
									}
								}

								if (string.IsNullOrEmpty(Get<string>(obj2, 8)) == false)
								{
									string[] deBuff = Get<string>(obj2, 8).Split(',');
									for (int b = 0; b < deBuff.Length; ++b)
									{
										BuffIcons bi = new BuffIcons(deBuff[b]);
										sde.deBuffIcons.Add(bi);
									}
								}

								m_DataMgr.m_SkillDataElementDic.Add(sde.ID, sde);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.CharacterData:
					#region CharacterData
					string[] CharacterDataSheet = { "Hero", "Mercenary", "Monster", "MiddleBoss", "Boss" };

					for (int i = 0; i < CharacterDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(CharacterDataSheet[i], DataPath);
						sheetName.Add(CharacterDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								CharacterDataElement cde = new CharacterDataElement();
								cde.ID = Get<int>(obj, 0);
								cde.CharacterName = Get<string>(obj, 1);
								cde.CharacterNameID = Get<string>(obj, 2);
								cde.CharacterDescID = Get<string>(obj, 3);
								cde.MoveSpeed = Get<float>(obj, 4);
								cde.Position = (StancePosition_E)Get<int>(obj, 5);
								cde.DoAerialUnitAttack = Get<int>(obj, 6) == 0 ? false : true;
								cde.Type = (DamageType_E)Get<int>(obj, 7);
								cde.AttackType = (AttackType_E)Get<int>(obj, 8);
								cde.AttackRange = Get<float>(obj, 9);
								cde.DamageTargetType = (DamageTargetType_E)Get<int>(obj, 10);
								cde.DamageTypeRange = Get<float>(obj, 11);
								cde.AttackSpeed = Get<float>(obj, 12);
								cde.AttackDamage = Get<int>(obj, 13);
								cde.HP = Get<int>(obj, 14);
								cde.PD = Get<int>(obj, 15);
								cde.MD = Get<int>(obj, 16);
								if (string.IsNullOrEmpty(Get<string>(obj, 17)) == false)
									cde.CriticalPer = Get<float>(obj, 17);
								if (string.IsNullOrEmpty(Get<string>(obj, 18)) == false)
									cde.CriticalDamage = Get<float>(obj, 18);

								cde.AllSkillList = new List<int>();
								if (string.IsNullOrEmpty(Get<string>(obj, 19)) == false)
								{
									string[] Skills = Get<string>(obj, 19).Split(",");
									for (int asl = 0; asl < Skills.Length; ++asl)
										cde.AllSkillList.Add(int.Parse(Skills[asl]));
								}
								if (string.IsNullOrEmpty(Get<string>(obj, 20)) == false)
									cde.Exp = Get<int>(obj, 20);
								if (string.IsNullOrEmpty(Get<string>(obj, 21)) == false)
									cde.Gold = Get<int>(obj, 21);

								cde.GrowHp = Get<int>(obj, 22);
								cde.GrowAttackDamage = Get<int>(obj, 23);
								cde.GrowPD = Get<int>(obj, 24);
								cde.GrowMD = Get<int>(obj, 25);
								if (string.IsNullOrEmpty(Get<string>(obj, 26)) == false)
									cde.GrowCriticalPer = Get<float>(obj, 26);
								if (string.IsNullOrEmpty(Get<string>(obj, 27)) == false)
									cde.GrowCriticalDamage = Get<float>(obj, 27);

								DataTable Character_FileTable = ReadSingleSheet(string.Format("{0}_File", CharacterDataSheet[i]), DataPath);
								//파일명 셋팅
								object[] obj2 = Character_FileTable.Rows[ii].ItemArray;

								cde.iconFileName = Get<string>(obj2, 2);
								cde.ObjectFileName = Get<string>(obj2, 3);
								cde.ObjectEffFileName = Get<string>(obj2, 4);

								m_DataMgr.m_CharacterDataElementDic.Add(cde.ID, cde);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.Ingame_CharacterGrowData:
					#region Ingame_CharacterGrowData
					string[] Ingame_CharacterGrowDataSheet = { "Hero", "Mercenary", "Monster", "MiddleBoss", "Boss" };

					InGame_CharacterGrowDataElement element = new InGame_CharacterGrowDataElement();
					for (int i = 0; i < Ingame_CharacterGrowDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(Ingame_CharacterGrowDataSheet[i], DataPath);
						sheetName.Add(Ingame_CharacterGrowDataSheet[i]);
						List<InGame_CharacterGrowData> DataList = new List<InGame_CharacterGrowData>();
						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								InGame_CharacterGrowData icgde = new InGame_CharacterGrowData();
								icgde.Level = Get<int>(obj, 0);
								icgde.MaxExp = Get<int>(obj, 1);
								icgde.Add_AttackDamage = Get<int>(obj, 2);
								icgde.Add_Hp = Get<int>(obj, 3);
								icgde.Add_PD = Get<int>(obj, 4);
								icgde.Add_MD = Get<int>(obj, 5);
								icgde.Add_CriticalPer = Get<float>(obj, 6);
								icgde.Add_CriticalDamage = Get<float>(obj, 7);
								DataList.Add(icgde);

							}
						}
						switch (i)
						{
							case 0:
								m_DataMgr.m_InGame_CharacterGrowDataElement.Hero = new List<InGame_CharacterGrowData>(DataList);
								break;
							case 1:
								m_DataMgr.m_InGame_CharacterGrowDataElement.Mercenary = new List<InGame_CharacterGrowData>(DataList);
								break;
							case 2:
								m_DataMgr.m_InGame_CharacterGrowDataElement.Monster = new List<InGame_CharacterGrowData>(DataList);
								break;
							case 3:
								m_DataMgr.m_InGame_CharacterGrowDataElement.MiddleBoss = new List<InGame_CharacterGrowData>(DataList);
								break;
							case 4:
								m_DataMgr.m_InGame_CharacterGrowDataElement.Boss = new List<InGame_CharacterGrowData>(DataList);
								break;
						}

						breakSheet++;
					}
					#endregion
					break;
				case EXls.UserSelectCard:
					#region UserSelectCard
					string[] UserSelectCardDataSheet = { "UserCard" };

					for (int i = 0; i < UserSelectCardDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(UserSelectCardDataSheet[i], DataPath);
						sheetName.Add(UserSelectCardDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								UserSelectCardDataElement ce = new UserSelectCardDataElement();
								ce.ID = Get<int>(obj, 0); ;
								ce.CardName = Get<string>(obj, 1);
								ce.CardNameID = Get<string>(obj, 2);
								ce.CardDescID = Get<string>(obj, 3);
								ce.CardRating = (Rating_E)Get<int>(obj, 4);
								ce.TargetSelect = (TargetSelect_E)Get<int>(obj, 5);

								for (int b = 6; b <= 10; ++b)
								{
									CardBuff card = new CardBuff();
									if (string.IsNullOrEmpty(Get<string>(obj, b)) == false)
									{
										card.Type = (CardBuffType_E)(b - 6);
										card.Value = Get<float>(obj, b);
										ce.CardBuffList.Add(card);
									}
								}

								ce.reBuyGoodsType = (Goods_E)Get<int>(obj, 11);
								ce.reBuyPrice = Get<int>(obj, 12);

								DataTable Character_FileTable = ReadSingleSheet("UserCard_File", DataPath);
								//파일명 셋팅
								object[] obj2 = Character_FileTable.Rows[ii].ItemArray;

								ce.CardIconName = Get<string>(obj2, 2);
								ce.BuffCardFileName = Get<string>(obj2, 3 + (int)ce.CardRating);
								//string ColorValue = Get<string>(obj2, Get<int>(obj, 4) + 3);
								//ColorUtility.TryParseHtmlString(ColorValue, out ce.ThisCardBuffColor);

								m_DataMgr.m_UserSelectCardDataElementDic.Add(ce.ID, ce);
							}
						}
						breakSheet++;
					}
					#endregion
					break;
				case EXls.UserSelectCardRandomTable:
					#region UserSelectCardRandomTable
					string[] UserSelectCardRandomTableDataSheet = { "UserCardTable" };

					for (int i = 0; i < UserSelectCardRandomTableDataSheet.Length; ++i)
					{
						DataTable Table = ReadSingleSheet(UserSelectCardRandomTableDataSheet[i], DataPath);
						sheetName.Add(UserSelectCardRandomTableDataSheet[i]);

						if (Table.Rows.Count > 0)
						{
							for (int ii = 0; ii < Table.Rows.Count; ii++)
							{
								breakRows = ii;
								object[] obj = Table.Rows[ii].ItemArray;

								if (string.IsNullOrEmpty(Get<string>(obj, 0)))
									continue;

								UserSelectCardTableDataElement ce = new UserSelectCardTableDataElement();
								ce.UserSelectCardID = Get<int>(obj, 0);
								ce.CardName = Get<string>(obj, 1);
								ce.Ratio = Get<float>(obj, 2);

								m_DataMgr.m_UserSelectCardTableDataElementList.Add(ce);
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

	static T Get<T>(object[] value, int index)
	{
		if (string.IsNullOrEmpty((string)Convert.ChangeType(value[index], typeof(string))))
			return default(T);

		return (T)Convert.ChangeType(value[index], typeof(T));
	}
}
