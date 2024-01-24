using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public GameObject[] companionPrefabs;
    public Transform companionRoot;

    readonly List<Hero> heroList = new();
    public List<Hero> HeroList => heroList;

    public Player player;
    public float spawnRange = 5.0f;
    
    public void CreateHero()
    {
        // Create a Random hero from companionPrefabs
        int randomIndex = Random.Range(0, companionPrefabs.Length);
        GameObject heroObj = Instantiate(
            companionPrefabs[randomIndex],
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
        Hero hero = heroObj.GetComponent<Hero>();
        hero.Initialize(player);
        heroList.Add(hero);
        
    }
}
