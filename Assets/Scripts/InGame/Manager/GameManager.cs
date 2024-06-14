using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    public float baseColliderWidth = 5.0f;

    public float heroFollowOffsetRange = 5.0f;

    HeroManager heroManager;
    EnemyManager enemyManager;

    public Vector3 characterRotation = new(-30.0f, 0.0f, 0.0f);

    public int playerCharacterId = 0;
    public int[] companionCharacterIds;

    public int[] userActiveSkillIds;

    Player player;
    public Player Player { get { return player; } }

    int companionPoints = 0;
    public int CompanionPoints { get { return companionPoints; } }
    float companionGauge = 0.0f;
    public float CompanionGauge { get { return companionGauge; } }
    float companionGaugeSpeed = 0.1f;

    int companionPointPerCycle = 1;

    int companionSummonPoint = 20;
    public int CompanionSummonPoint { get { return companionSummonPoint; } }
    int companionSummonPointIncrease = 1;

    int companionCallPoint = 20;
    public int CompanionCallPoint { get { return companionCallPoint; } }

    float gameTimer = 0.0f;
    public float GameTime { get { return gameTimer; } }

    int playerExp = 0;
    public int PlayerExp { get { return playerExp; } }
    public int MaxExp {
        get
        {
            List<InGame_CharacterGrowData> growList = DataMgr.instance.GetGrowData(player.CharacterInfo.ID);
            if (this.playerLevel > growList.Count)
            {
                return 0;
            }
            InGame_CharacterGrowData growData = growList[this.playerLevel];
            return growData.MaxExp;
        }
    }

    int playerLevel = 0;
    public int PlayerLevel { get { return playerLevel; } }

    Dictionary<int, int> companionLevelMap = new ();
    public int GetCompanionLevel(int characterId)
    {
        return companionLevelMap[characterId];
    }

    bool isBuffCardSelecting = false;
    int levelUpBuffToSelectCount = 0;

    List<UserSelectCardDataElement> buffCardList = new();
    public List<UserSelectCardDataElement> BuffCardList => buffCardList;

    void Awake()
    {
        heroManager = GetComponent<HeroManager>();
        enemyManager = GetComponent<EnemyManager>();

        player = heroManager.CreatePlayer(playerCharacterId);

        companionPoints = DataMgr.instance.m_InGameSystemElement.GetMercenaryPoint;

        companionGaugeSpeed = 1.0f / DataMgr.instance.m_InGameSystemElement.MercenaryPointGetTime;

        companionPointPerCycle = DataMgr.instance.m_InGameSystemElement.GetMercenaryPoint;

        companionSummonPoint = DataMgr.instance.m_InGameSystemElement.Summon_NeedPoint;
        companionSummonPointIncrease = 0;

        companionCallPoint = DataMgr.instance.m_InGameSystemElement.Call_NeedPoint;

        foreach (int characterId in companionCharacterIds)
        {
            companionLevelMap[characterId] = 0;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    void Update()
    {
        // companionGauge += companionGaugeSpeed * Time.deltaTime;
        // if (companionGauge >= 1.0f)
        // {
        //     companionGauge = 0.0f;
        //     companionPoints += companionPointPerCycle;
        // }

        gameTimer += Time.deltaTime;
    }

    public void SummonCompanion()
    {
        if (companionPoints >= companionSummonPoint)
        {
            companionPoints -= companionSummonPoint;
            companionSummonPoint += companionSummonPointIncrease;
            Hero hero = heroManager.CreateHero();

            foreach (CompanionUI companionUI in UIManager.instance.companionUIs)
            {
                // if (companionUI.CharacterInfo.ID == hero.CharacterInfo.ID)
                // {
                //     companionUI.CreateSummonEffect();
                //     break;
                // }
            }
        }        
    }

    public void CallCompanion()
    {
        if (companionPoints >= companionCallPoint)
        {
            companionPoints -= companionCallPoint;
            player.OnFollowCall();
            foreach (Hero hero in heroManager.AliveHeroList)
            {
                hero.FollowPlayer();
            }
        }
    }

    public List<Character> GetEnemyList()
    {
        return enemyManager.AliveEnemyList.Cast<Character>().ToList();
    }

    public List<Character> GetHeroList(bool includePlayer)
    {
        List<Character> heroList = new();
        if (includePlayer)
        {
            heroList.Add(player);
        }
        heroList.AddRange(heroManager.AliveHeroList.Cast<Character>());
        return heroList;
    }

    public void AddPlayerExp(int addExp)
    {
        playerExp += addExp;
        if (playerExp >= MaxExp)
        {
            playerExp -= MaxExp;
            playerLevel++;

            // if (!isBuffCardSelecting)
            // {
            //     StartCoroutine(BuffCardSelectCoroutine());
            // }
            // isBuffCardSelecting = true;
            // levelUpBuffToSelectCount++;

            player.OnLevelUp(playerLevel);
        }
    }

    public void CompanionLevelUp(int characterId)
    {
        int level = companionLevelMap[characterId];
        int needPoint = DataMgr.instance.m_InGameSystemElement.Mercenary_LevelUp_NeedPoint[level];
        if (companionPoints >= needPoint)
        {
            companionPoints -= needPoint;
            companionLevelMap[characterId]++;

            foreach (Hero hero in HeroManager.instance.AliveHeroList)
            {
                if (hero.CharacterInfo.ID == characterId)
                {
                    hero.OnLevelUp(level + 1);
                }
            }
        }
    }

    IEnumerator BuffCardSelectCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (levelUpBuffToSelectCount > 0)
        {
            List<UserSelectCardDataElement> cardList = DataMgr.instance.GetUserSelectCardList();
            UIManager.instance.buffSelectUI.Initialize(cardList);
            PauseGame();
        }

        while (levelUpBuffToSelectCount > 0)
        {
            if (!UIManager.instance.buffSelectUI.gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(0.1f);
                List<UserSelectCardDataElement> cardList = DataMgr.instance.GetUserSelectCardList();
                UIManager.instance.buffSelectUI.Initialize(cardList);
                PauseGame();
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        isBuffCardSelecting = false;
    }

    public void OnSelectBuffCard(UserSelectCardDataElement cardData)
    {
        foreach (CardBuff cardBuff in cardData.CardBuffList)
        {
            if (cardBuff.Type == CardBuffType_E.Heal)
            {
                List<Character> targetList = new ();
                if (cardData.TargetSelect == TargetSelect_E.One || cardData.TargetSelect == TargetSelect_E.All)
                {
                    targetList.Add(player);
                }
                if (cardData.TargetSelect == TargetSelect_E.Area || cardData.TargetSelect == TargetSelect_E.All)
                {
                    targetList.AddRange(heroManager.AliveHeroList.Cast<Character>());
                }
                foreach (Character target in targetList)
                {
                    target.Heal(cardBuff.Value);
                }
            }
        }
        buffCardList.Add(cardData);
        levelUpBuffToSelectCount--;
        UIManager.instance.buffSelectUI.gameObject.SetActive(false);
        ResumeGame();
    }
}
