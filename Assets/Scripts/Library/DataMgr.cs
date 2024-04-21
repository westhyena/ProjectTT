using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;

#region Enums

public enum Rating_E
{
	Normal,
	HighGrade,
	Rare,
	Epic
}

public enum Goods_E
{
	Gold,
	Cash
}

public enum ActivePosition_E
{
	/// <summary>
	/// 타겟
	/// </summary>
	Target,
	/// <summary>
	/// 시전자
	/// </summary>
	Me,
}

public enum Target_E
{
	/// <summary>
	/// 적
	/// </summary>
	Enemy,
	/// <summary>
	/// 아군
	/// </summary>
	Our
}

public enum DamageType_E
{
	/// <summary>
	/// 물리
	/// </summary>
	Physics,
	/// <summary>
	/// 마법
	/// </summary>
	Magic
}

public enum TargetSelect_E
{
	/// <summary>
	/// 단일
	/// </summary>
	One,
	/// <summary>
	/// 범위
	/// </summary>
	Area,
	/// <summary>
	/// 전체
	/// </summary>
	All
}

public enum StancePosition_E
{
	Ground,
	Fly
}

public enum AttackType_E
{
	/// <summary>
	/// 근거리
	/// </summary>
	Melee,
	/// <summary>
	/// 원거리
	/// </summary>
	Ranged
}

public enum DamageTargetType_E
{
	/// <summary>
	/// 단일
	/// </summary>
	One,
	/// <summary>
	/// 범위
	/// </summary>
	Area,
	/// <summary>
	/// 공격범위 절반크기
	/// </summary>
	Range_HalfArea
}

public enum MonsterType_E
{
	/// <summary>
	/// 일반
	/// </summary>
	Normal,
	/// <summary>
	/// 중간보스
	/// </summary>
	MiddleBoss,
	/// <summary>
	/// 보스
	/// </summary>
	Boss
}

#endregion

#region 스킬정보 베이스

[Serializable]
public class SkillDataBase
{
	public SkillDataKind_E SkillDataKind;
	public float Value;
	public float Time;

	public SkillDataBase(int _SkillDataKind, float _Value, float _Time = 0f)
	{
		SkillDataKind = (SkillDataKind_E)_SkillDataKind;
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
	/// 몬스터 타입
	/// </summary>
	public MonsterType_E MonsterType;
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
	/// 시작 용병 포인트
	/// </summary>
	public int StartMercenaryPoint;
	/// <summary>
	/// 스테이지 웨이브 정보 
	/// </summary>
	public List<WaveDataInfo> Wave0 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave1 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave2 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave3 = new List<WaveDataInfo>();
	public List<WaveDataInfo> Wave4 = new List<WaveDataInfo>();

	/// <summary>
	/// 맵 아이콘 파일명
	/// </summary>
	public string IconName;
	/// <summary>
	/// 맵 오브젝트 파일명
	/// </summary>
	public string StageMapObjectName;
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
	public Rating_E Rating;
	/// <summary>
	/// 사용될 재화 타입
	/// </summary>
	public Goods_E PriceType;
	/// <summary>
	/// 필요 재화갯수
	/// </summary>
	public int PriceValue;
	/// <summary>
	/// 시전 위치
	/// </summary>
	public ActivePosition_E ActivePosition;
	/// <summary>
	/// 대상
	/// </summary>
	public Target_E Target;
	/// <summary>
	/// 쿨타임 (초)
	/// </summary>
	public float CoolTime;
	/// <summary>
	/// 타입
	/// </summary>
	public DamageType_E Type;
	/// <summary>
	/// 시전타입
	/// </summary>
	public TargetSelect_E TargetSelectType;
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
	/// </summary>
	public ActivePosition_E ActivePosition;
	/// <summary>
	/// 대상 구분
	/// </summary>
	public Target_E Target;
	/// <summary>
	/// 쿨타임 (초)
	/// </summary>
	public float CoolTime;
	/// <summary>
	/// 데미지 타입
	/// </summary>
	public DamageType_E Type;
	/// <summary>
	/// 시전타입
	/// </summary>
	public TargetSelect_E TargetSelectType;
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

#region CharacterData

[Serializable]
public class CharacterDataElement
{
	public int ID;
	public string CharacterName;
	public string CharacterNameID;
	public string CharacterDescID;

	public float MoveSpeed;
	/// <summary>
	/// 유닛 스텐스( 지상이냐 공중이냐 )
	/// </summary>
	public StancePosition_E Position;
	/// <summary>
	/// 공격타입
	/// </summary>
	public DamageType_E Type;
	/// <summary>
	/// 공격 타입
	/// </summary>
	public AttackType_E AttackType;
	/// <summary>
	/// 공격 범위
	/// </summary>
	public float AttackRange;
	/// <summary>
	/// 공격대상 구분 범위
	/// </summary>
	public DamageTargetType_E DamageTargetType;
	/// <summary>
	/// 타겟에 도착후 범위 공격 범위
	/// DamageType이 1일때만 유효
	/// </summary>
	public float DamageTypeRange;
	public float AttackSpeed;
	public int AttackDamage;
	public int HP;
	/// <summary>
	/// 물리 방어력
	/// </summary>
	public int PD;
	/// <summary>
	/// 마법 방어력
	/// </summary>
	public int MD;
	/// <summary>
	/// 영웅별 전체 스킬 리스트
	/// </summary>
	public List<int> AllSkillList = new List<int>();
	/// <summary>
	/// 지급 경험치 (몬스터만 유효)
	/// </summary>
	public int Exp;
	/// <summary>
	/// 지급 골드 (몬스터만 유효)
	/// </summary>
	public int Gold;

	public string iconFileName;
	/// <summary>
	/// 오브젝트 파일명
	/// </summary>
	public string ObjectFileName;
	/// <summary>
	/// 투사체 파일명
	/// </summary>
	public string ObjectEffFileName;

}

#endregion

#region InGame_ChracterGrowData
[Serializable]
public class InGame_CharacterGrowData
{
	public int Level;
	/// <summary>
	/// 현재레벨의 Max경험치
	/// </summary>
	public int MaxExp;
	/// <summary>
	/// 추가되는 공격력
	/// </summary>
	public int Add_AttackDamage;
	/// <summary>
	/// 추가되는 체력
	/// </summary>
	public int Add_Hp;
	/// <summary>
	/// 추가되는 물방
	/// </summary>
	public int Add_PD;
	/// <summary>
	/// 추가되는 마방
	/// </summary>
	public int Add_MD;
}

[Serializable]
public class InGame_CharacterGrowDataElement
{
	/// <summary>
	/// 영웅
	/// </summary>
	public List<InGame_CharacterGrowData> Hero = new List<InGame_CharacterGrowData>();
	/// <summary>
	/// 용병
	/// </summary>
	public List<InGame_CharacterGrowData> Mercenary = new List<InGame_CharacterGrowData>();
	/// <summary>
	/// 몬스터
	/// </summary>
	public List<InGame_CharacterGrowData> Monster = new List<InGame_CharacterGrowData>();
	/// <summary>
	/// 중간보스
	/// </summary>
	public List<InGame_CharacterGrowData> MiddleBoss = new List<InGame_CharacterGrowData>();
	/// <summary>
	/// 보스
	/// </summary>
	public List<InGame_CharacterGrowData> Boss = new List<InGame_CharacterGrowData>();
}

#endregion

#region UserSelectCard_E

public enum CardBuffType_E
{
	Critical_Percent,
	Critical_Damage,
	AttackUp_Percent,
	AttackSpeed_Percent,
	Heal
}

[Serializable]
public struct CardBuff
{
	/// <summary>
	/// 적용 버프
	/// </summary>
	public CardBuffType_E Type;
	public float Value;
}

[Serializable]
public class UserSelectCardDataElement
{
	public int ID;
	public string CardName;
	public string CardNameID;
	public string CardDescID;
	/// <summary>
	/// 카드 등급
	/// </summary>
	public Rating_E CardRating;
	/// <summary>
	/// 대상
	/// </summary>
	public TargetSelect_E TargetSelect;
	public List<CardBuff> CardBuffList = new List<CardBuff>();
	/// <summary>
	/// 카드 아이콘
	/// </summary>
	public string CardIconName;
	/// <summary>
	/// 카드 등급 컬러 (현재 등급에 맞게 셋팅되있음)
	/// </summary>
	public Color ThisCardBuffColor = new Color();

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

	/// <summary>
	/// 스킬 정보
	/// </summary>
	public SkillDataElementDic m_SkillDataElementDic = new SkillDataElementDic();

	/// <summary>
	/// 케릭터 정보
	/// </summary>
	public CharacterDataElementDic m_CharacterDataElementDic = new CharacterDataElementDic();

	/// <summary>
	/// 케릭터 성장 정보
	/// </summary>
	public InGame_CharacterGrowDataElement m_InGame_CharacterGrowDataElement = new InGame_CharacterGrowDataElement();

	/// <summary>
	/// 영웅 레벨업 유저 셀렉트 카드
	/// </summary>
	public UserSelectCardDataElementDic m_UserSelectCardDataElementDic = new UserSelectCardDataElementDic();
	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);

		Debug.Log(m_UserActiveSkillDataElementDic[0].UserSkillName);


	}

	/// <summary>
	/// 최종 데미지
	/// </summary>
	/// <param name="Damage">데미지</param>
	/// <param name="DamageType">데미지 타입</param>
	/// <param name="TargetIndex">대상 CharacterIndex</param>
	/// <param name="TargetLevel">대상 레벨</param>
	/// <returns></returns>
	public int GetFinalDamage(int Damage,DamageType_E DamageType,int TargetIndex,int TargetLevel)
	{
		int returnDamage = 0;
		float RValue = 0.1f;
		List<InGame_CharacterGrowData> TargetGrowDataList = GetGrowData(TargetIndex);
		switch (DamageType)
		{
			case DamageType_E.Physics:
				int TargetMaxPD = m_CharacterDataElementDic[TargetIndex].PD + TargetGrowDataList[TargetGrowDataList.Count - 1].Add_PD; // 타겟의 최고레벨의 방어력을 가져옴
				int TargetPD = m_CharacterDataElementDic[TargetIndex].PD + TargetGrowDataList[TargetLevel].Add_PD; // 타겟의 현재 방어력
				float TotalPDValue = ((float)TargetPD / (float)TargetMaxPD) * RValue; // 저항값을 구하기위한 최대방어력 대비 비율
				int MaxresistancePValue = (int)(Damage * TotalPDValue); //데미지에 비례한 최대 저항값
				int RndMaxResistancePValue = UnityEngine.Random.Range(0, MaxresistancePValue); // 0~저항값 사이 랜덤값
				returnDamage = Damage - (TargetPD + RndMaxResistancePValue); //기본 방어 + 랜덤한 저항값으로 데미지에서 상쇄시켜준다.
				break;
			case DamageType_E.Magic:
				int TargetMaxMD = m_CharacterDataElementDic[TargetIndex].MD + TargetGrowDataList[TargetGrowDataList.Count - 1].Add_MD;
				int TargetMD = m_CharacterDataElementDic[TargetIndex].MD + TargetGrowDataList[TargetLevel].Add_MD;
				float TotalMDValue = ((float)TargetMD / (float)TargetMaxMD) * RValue;
				int resistanceMValue = (int)(Damage * TotalMDValue);
				int RndMaxResistanceMValue = UnityEngine.Random.Range(0, resistanceMValue);
				returnDamage = Damage - (TargetMD + RndMaxResistanceMValue);
				break;
			default:
				return Damage;
		}

		return returnDamage <= 0 ? 1 : returnDamage;
	}

	/// <summary>
	/// 해당 Character 타입의 성장 테이블을 가져온다.
	/// </summary>
	/// <param name="CharacterIndex">대상의 CharacterIndex</param>
	/// <returns></returns>
	public List<InGame_CharacterGrowData> GetGrowData(int CharacterIndex)
	{
		if (CharacterIndex >= 0 && CharacterIndex < 100) //hero
			return m_InGame_CharacterGrowDataElement.Hero;
		else if (CharacterIndex >= 100 && CharacterIndex < 1000) //Mercenary
			return m_InGame_CharacterGrowDataElement.Mercenary;
		else if (CharacterIndex >= 1000 && CharacterIndex < 2000) //Monster
			return m_InGame_CharacterGrowDataElement.Monster;
		else if (CharacterIndex >= 2000 && CharacterIndex < 3000) //MiddleBoss
			return m_InGame_CharacterGrowDataElement.MiddleBoss;
		else if (CharacterIndex >= 3000 && CharacterIndex < 4000) //Boss
			return m_InGame_CharacterGrowDataElement.Boss;

		return null;
	}

	private static DataMgr _instance;
	public static DataMgr instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<DataMgr>();
			}
			return _instance;
		}
	}

	public CharacterDataElement GetCharacterDataElement(int id)
	{
		if (m_CharacterDataElementDic.ContainsKey(id))
		{
			return m_CharacterDataElementDic[id];
		}
		else
		{
			Debug.LogError("CharacterDataElementDic에 없는 ID입니다. ID : " + id);
			return null;
		}
	}
}
