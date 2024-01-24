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

    HeroManager heroManager;
    EnemyManager enemyManager;

    void Awake()
    {
        heroManager = GetComponent<HeroManager>();
        enemyManager = GetComponent<EnemyManager>();
    }

    public Hero GetNearestHero(Vector2 position)
    {
        Hero nearestHero = null;
        float nearestDistanceSqr = Mathf.Infinity;
        foreach (Hero hero in heroManager.HeroList)
        {
            float distanceSqr = Vector2.SqrMagnitude(position - hero.Position2D);
            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestHero = hero;
            }
        }
        return nearestHero;
    }
}
