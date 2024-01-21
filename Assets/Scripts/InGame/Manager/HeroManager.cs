using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public GameObject[] companionPrefabs;
    public Transform companionRoot;

    public Player player;
    public float spawnRange = 5.0f;
    
    public void CreateHero()
    {
        // Create a Random hero from companionPrefabs
        int randomIndex = Random.Range(0, companionPrefabs.Length);
        GameObject hero = Instantiate(
            companionPrefabs[randomIndex],
            player.transform.position,
            Quaternion.identity
        );
        hero.transform.parent = companionRoot;
        // Hero randomly spawn around the player
        Vector2 randomPosition = Random.insideUnitCircle.normalized * spawnRange;
        hero.transform.position = player.transform.position + new Vector3(
            randomPosition.x,
            randomPosition.y,
            0.0f
        );
        hero.GetComponent<Hero>().Initialize(player);   
    }
}
