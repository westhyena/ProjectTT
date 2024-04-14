using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;

#region 스킬정보 베이스

[Serializable]
public class SkillDataBase
{
	public int SkillDataKind;
	public int Value;
	public float Time;

	public SkillDataBase(int _SkillDataKind,int _Value,float _Time = 0f)
	{
		SkillDataKind = _SkillDataKind;
		Value = _Value;
		Time = _Time;
	}
}

public enum SkillDataKind_E
{
	/// <summary>
	/// 데미지
	/// </summary>
	Damage,
	/// <summary>
	/// 도트 데미지
	/// </summary>
	DotDamage,
	/// <summary>
	/// 공격력 증가
	/// </summary>
	AttackUp,
	/// <summary>
	/// 공격 속도 증가
	/// </summary>
	AttackSpeedUp,
	/// <summary>
	/// 힐
	/// </summary>
	Heal,
	/// <summary>
	/// 무적
	/// </summary>
	Immunity,
	/// <summary>
	/// 스턴
	/// </summary>
	Stun,
	/// <summary>
	/// 공격속도 감소
	/// </summary>
	AttackSpeedDown,
};
#endregion

#region Translation
[Serializable]
public class TranslationElement
{
  public string ID;
  public string Kor;
  public string Eng;
  public string Jpn;
  public string Prt;//포르투갈
  public string Deu;//독일
  public string Rus;//러시아
  public string Fra;//프랑스
  public string Chn_s;//중 간체
  public string Chn_t;//중 번체
  public string Esp; //스페인
  public string Tha; //태국
  public string Vnm;//베트남
  public string Mys;//말레이시아
}
#endregion

#region InGameSystemElement
[Serializable]
public class InGameSystemElement
{
	/// <summary>
	/// 용병포인트 획득 시간
	/// </summary>
	public float MercenaryPointGetTime;
	/// <summary>
	/// 용병포인트 획득량
	/// </summary>
	public int GetMercenaryPoint;
	/// <summary>
	/// 용병 레벨업 필요 포인트 ( 레벨 = index )
	/// </summary>
	public List<int> Mercenary_LevelUp_NeedPoint = new List<int>();
	/// <summary>
	/// 소환 필요 포인트
	/// </summary>
	public int Summon_NeedPoint;
	/// <summary>
	/// 소집 필요 포인트
	/// </summary>
	public int Call_NeedPoint;
}
#endregion

#region StageWaveData

[Serializable]
public struct WaveDataInfo
{
	/// <summary>
	/// 웨이브
	/// </summary>
	public int WaveID;
	/// <summary>
	/// 등장시간(초)
	/// </summary>
	public float SummonTime;
	/// <summary>
	/// (구분용) 소환 오브젝트 이름
	/// </summary>
	public string ObjName;
	/// <summary>
	/// 소환 오브젝트 ID
	/// </summary>
	public int CharacterID;
	/// <summary>
	/// 소환 오브젝트 레벨
	/// </summary>
	public int CharacterLevel;
	/// <summary>
	/// 소환될 갯수
	/// </summary>
	public int SummonCount;
}

[Serializable]
public class StageWaveDataElement
{
	/// <summary>
	/// 스테이지 번호
	/// </summary>
	public int id;
	/// <summary>
	/// 스테이지 이름 (구분용)
	/// </summary>
	public string StageName;
	/// <summary>
	/// 스테이지 이름 Translation ID
	/// </summary>
	public string StageNameID;
	/// <summary>
	/// 스테이지 설명 Translation ID
	/// </summary>
	public string StageDescID;
	/// <summary>
	/// 제한시간
	/// </summary>
	public float PlayTime;
	/// <summary>
	/// 스테이지 웨이브 정보 
	/// </summary>
	public List<WaveDataInfo> Wave0 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave1 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave2 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave3 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave4 = new List<WaveDataInfo>();
}

#endregion

#region UserActiveSkill

[Serializable]
public class UserActiveSkillDataElement
{
	public int ID;
	public string UserSkillName;
	public string UserSkillNameID;
	public string UserSkillDescID;
	/// <summary>
	/// 등급
	/// </summary>
	public int Rating;
	/// <summary>
	/// 사용될 재화 타입
	/// 0 골드
	/// 1 캐시재화(다이아)
	/// </summary>
	public int PriceType;
	/// <summary>
	/// 필요 재화갯수
	/// </summary>
	public int PriceValue;
	/// <summary>
	/// 시전 위치
	/// 0 타겟
	/// 1 자기자신
	/// </summary>
	public int ActivePosition;
	/// <summary>
	/// 0 적
	/// 1 아군
	/// </summary>
	public int Target;
	/// <summary>
	/// 쿨타임 (초)
	/// </summary>
	public float CoolTime;
	/// <summary>
	/// 타입
	/// 0 물리
	/// 1 마법
	/// </summary>
	public int Type;
	/// <summary>
	/// 시전타입
	/// 0 단일
	/// 1 범위
	/// 2 전체
	/// </summary>
	public int TargetSelectType;
	/// <summary>
	/// 범위일때 ActivePosition의 기준에서의 범위
	/// </summary>
	public float DamageTypeRange;

	public List<SkillDataBase> SkillData = new List<SkillDataBase>();
	//public Dictionary<SkillDataKind_E, SkillDataBase> SkillData = new Dictionary<SkillDataKind_E, SkillDataBase>();

	public string IconName;
	/// <summary>
	/// 스킬 이펙트
	/// </summary>
	public string Eff_SkillName;
}

#endregion

#region SkillData

[Serializable]
public class SkillDataElement
{
	public int ID;
	public string UserSkillName;
	public string UserSkillNameID;
	public string UserSkillDescID;

	/// <summary>
	/// 시전 위치
	/// 0 타겟
	/// 1 자기자신
	/// </summary>
	public int ActivePosition;
	/// <summary>
	/// 0 적
	/// 1 아군
	/// </summary>
	public int Target;
	/// <summary>
	/// 쿨타임 (초)
	/// </summary>
	public float CoolTime;
	/// <summary>
	/// 타입
	/// 0 물리
	/// 1 마법
	/// </summary>
	public int Type;
	/// <summary>
	/// 시전타입
	/// 0 단일
	/// 1 범위
	/// 2 전체
	/// </summary>
	public int TargetSelectType;
	/// <summary>
	/// 범위일때 ActivePosition의 기준에서의 범위
	/// </summary>
	public float DamageTypeRange;

	public List<SkillDataBase> SkillData = new List<SkillDataBase>();
	//public Dictionary<SkillDataKind_E, SkillDataBase> SkillData = new Dictionary<SkillDataKind_E, SkillDataBase>();

	public string IconName;
	/// <summary>
	/// 스킬 이펙트
	/// </summary>
	public string Eff_SkillName;
	/// <summary>
	/// 피격 이펙트
	/// </summary>
	public string Eff_Name;
}

#endregion


#region 쿠폰
[System.Serializable]
public class PromotionCodeElement
{
  public string Code;
  public string UserID;
  public int InAppItemID;
  public string ImageName;
}

#endregion

#region 버전
[System.Serializable]
public class VersionElement
{
  public float AppVersion;
  public int XlsVersion;
  public int ADCount;
  public int ADProb;
}

#endregion

public class DataMgr : MonoBehaviour
{
	/// <summary>
	/// 로컬라인징 text
	/// </summary>
    public TranslationElementDic m_TranslationElementDic = new TranslationElementDic();
	/// <summary>
	/// 인게임 정보 (용병 포인트 사용량)
	/// </summary>
    public InGameSystemElement m_InGameSystemElement = new InGameSystemElement();
	/// <summary>
	/// 스테이지 정보 (웨이브 포함)
	/// </summary>
	public StageWaveDataElementDic m_StageWaveDataElementDic = new StageWaveDataElementDic();
	/// <summary>
	/// 유저 엑티브 스킬 정보
	/// </summary>
	public UserActiveSkillDataElementDic m_UserActiveSkillDataElementDic = new UserActiveSkillDataElementDic();

	public SkillDataElementDic m_SkillDataElementDic = new SkillDataElementDic();
	private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

		Debug.Log( m_UserActiveSkillDataElementDic[0].UserSkillName);
    }
}
