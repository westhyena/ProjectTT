using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    private static HeroManager _instance;
    public static HeroManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<HeroManager>();
            }
            return _instance;
        }
    }

    public GameObject[] companionPrefabs;
    public Transform companionRoot;

    readonly List<Hero> heroList = new();
    public List<Hero> HeroList => heroList;
    public List<Hero> AliveHeroList => heroList.FindAll(hero => !hero.IsDead);

    public Player player;
    public float spawnRange = 5.0f;

    void Awake()
    {
        foreach (GameObject prefab in companionPrefabs)
        {
            CreateHero(prefab);
        }
    }

    Hero CreateHero(GameObject prefab)
    {
        GameObject heroObj = Instantiate(
            prefab,
            player.transform.position,
            Quaternion.identity
        );
        heroObj.transform.parent = companionRoot;
        // Hero randomly spawn around the player
        Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRange;
        heroObj.transform.position = player.transform.position + new Vector3(
            randomPosition.x,
            randomPosition.y,
            0.0f
        );
        heroObj.transform.localRotation = Quaternion.Euler(GameManager.instance.characterRotation);
        Hero hero = heroObj.GetComponent<Hero>();
        hero.Initialize(player);
        heroList.Add(hero);
        return hero;
    }

    public Hero CreateHero()
    {
        // Create a Random hero from companionPrefabs
        int randomIndex = Random.Range(0, companionPrefabs.Length);
        return CreateHero(companionPrefabs[randomIndex]);
    }
}
