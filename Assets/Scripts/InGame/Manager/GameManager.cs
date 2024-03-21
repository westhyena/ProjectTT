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

    public float defenceFactor1 = 300.0f;
    public float defenceFactor2 = 3.0f;
    public float criticalFactor = 1.5f;

    // For test, 추후엔 Data Table에서 ㄷ불러오게 변경
    public float playerAttackCooltime = 3.0f;

    public float heroAttackRangeStartDistance = 20.0f;
    public float heroAttackCooltime = 3.0f;
    public float heroFollowOffsetRange = 5.0f;
    public float heroFollowSpeed = 50.0f;

    public float enemyAttackCooltime = 3.0f;

    HeroManager heroManager;
    EnemyManager enemyManager;

    public Vector3 characterRotation = new Vector3(-30.0f, 0.0f, 0.0f);

    Player player;
    public Player Player { get { return player; } }

    int companionPoints = 0;
    public int CompanionPoints { get { return companionPoints; } }
    float companionGauge = 0.0f;
    public float CompanionGauge { get { return companionGauge; } }
    float companionGaugeSpeed = 0.1f;

    public int COMPANION_SUMMON_POINT = 1;

    float gameTimer = 0.0f;
    public float GameTime { get { return gameTimer; } }

    void Awake()
    {
        DataManager inst = DataManager.instance;
        heroManager = GetComponent<HeroManager>();
        enemyManager = GetComponent<EnemyManager>();

        player = heroManager.CreatePlayer();
    }

    void Update()
    {
        companionGauge += companionGaugeSpeed * Time.deltaTime;
        if (companionGauge >= 1.0f)
        {
            companionGauge = 0.0f;
            companionPoints++;
        }

        gameTimer += Time.deltaTime;
    }

    public void SummonCompanion()
    {
        if (companionPoints >= COMPANION_SUMMON_POINT)
        {
            companionPoints -= COMPANION_SUMMON_POINT;
            heroManager.CreateHero();
        }        
    }

    public Character GetRandomHero()
    {
        int randIdx = Random.Range(0, heroManager.HeroList.Count + 1);
        if (randIdx == heroManager.HeroList.Count)
        {
            return player;
        }
        return heroManager.HeroList[randIdx];
    }

    public Character GetRandomEnemy()
    {
        int randIdx = Random.Range(0, enemyManager.EnemyList.Count);
        return enemyManager.EnemyList[randIdx];
    }

    public Character GetNearestHero(Vector2 position)
    {
        return GetNearestHero(position, true);
    }

    public Character GetNearestHero(Vector2 position, bool includePlayer)
    {
        Character nearestTarget = null;
        float nearestDistanceSqr = Mathf.Infinity;
        foreach (Hero hero in heroManager.AliveHeroList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - hero.Position2D);
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = hero;
            }
        }

        if (includePlayer)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - player.Position2D);
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestTarget = player;
            }
        }
        return nearestTarget;
    }

    public Character GetNearestEnemy(Vector2 position)
    {
        Enemy nearestEnemy = null;
        float nearestDistanceSqr = Mathf.Infinity;
        foreach (Enemy enemy in enemyManager.AliveEnemyList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - enemy.Position2D);
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
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
}
