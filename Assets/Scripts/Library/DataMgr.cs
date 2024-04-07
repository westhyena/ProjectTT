using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;
[System.Serializable]
public class TranslationElement
{
  public ObscuredInt ID;
  public ObscuredString Kor;
  public ObscuredString Eng;
  public ObscuredString Jpn;
  public ObscuredString Prt;//포르투갈
  public ObscuredString Deu;//독일
  public ObscuredString Rus;//러시아
  public ObscuredString Fra;//프랑스
  public ObscuredString Chn_s;//중 간체
  public ObscuredString Chn_t;//중 번체
  public ObscuredString Esp; //스페인
  public ObscuredString Tha; //태국
  public ObscuredString Vnm;//베트남
  public ObscuredString Mys;//말레이시아
}

#region 쿠폰
[System.Serializable]
public class PromotionCodeElement
{
  public ObscuredString Code;
  public ObscuredString UserID;
  public ObscuredInt InAppItemID;
  public ObscuredString ImageName;
}

#endregion

#region 버전
[System.Serializable]
public class VersionElement
{
  public ObscuredFloat AppVersion;
  public ObscuredInt XlsVersion;
  public ObscuredInt ADCount;
  public ObscuredInt ADProb;
}

#endregion

public class DataMgr : MonoBehaviour
{
  public TranslationElementDic m_TranslationElementDic = new TranslationElementDic();


  private void Awake()
  {
    DontDestroyOnLoad(this.gameObject);
    SceneManager.LoadScene("Logo_Start");
  }
}
