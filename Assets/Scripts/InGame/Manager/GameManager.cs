using System.Collections;
using System.Collections.Generic;
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

    // For test, 추후엔 Data Table에서 ㄷ불러오게 변경
    public float playerMovementSpeed = 20.0f;
    public float heroMovementSpeed = 20.0f;
    public float enemyMovementSpeed = 20.0f;

    public float heroTargetStartDistance = 20.0f;

    public float enemyTargetStartDistance = 20.0f;
    public float enemyAttackStartDistance = 5.0f;
    public float enemyAttackCooltime = 3.0f;

    HeroManager heroManager;
    EnemyManager enemyManager;

    public Vector3 characterRotation = new Vector3(-30.0f, 0.0f, 0.0f);

    public Player player;

    void Awake()
    {
        heroManager = GetComponent<HeroManager>();
        enemyManager = GetComponent<EnemyManager>();
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
        foreach (Hero hero in heroManager.HeroList)
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
        foreach (Enemy enemy in enemyManager.EnemyList)
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
}
