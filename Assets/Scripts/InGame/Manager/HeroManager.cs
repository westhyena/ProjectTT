using System.Collections;
using System.Collections.Generic;
using System.Resources;
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

    public class HeroInfo
    {
        public string name;
        public string PrefabName;
        public string IconName;
    }

    public string playerCharacterId = "10001";

    public string[] companionCharacterIds;
    CharacterInfo[] companionCharacterInfos;
    public CharacterInfo[] CompanionCharacterInfos => companionCharacterInfos;

    public Transform companionRoot;

    readonly List<Hero> heroList = new();
    public List<Hero> HeroList => heroList;
    public List<Hero> AliveHeroList => heroList.FindAll(hero => !hero.IsDead);

    public float spawnRange = 5.0f;

    void Start()
    {
        companionCharacterInfos = new CharacterInfo[companionCharacterIds.Length];
        for (int i = 0; i < companionCharacterIds.Length; ++i)
        {
            companionCharacterInfos[i] = DataManager.instance.GetCharacterInfo(companionCharacterIds[i]);
        }
    }

    public Player CreatePlayer()
    {
        CharacterInfo info = DataManager.instance.GetCharacterInfo(playerCharacterId);
        GameObject prefab = ResourceManager.GetCharacterPrefab(info.prefabKey);
        GameObject playerObj = Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.Euler(GameManager.instance.characterRotation)
        );
        Player player = playerObj.AddComponent<Player>();
        player.InitializeCharacter(playerCharacterId);
        return player;
    }

    Hero CreateHero(CharacterInfo info)
    {
        Player player = GameManager.instance.Player;
        GameObject prefab = ResourceManager.GetCharacterPrefab(info.prefabKey);
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
        Hero hero = heroObj.AddComponent<Hero>();
        hero.Initialize(player);
        hero.InitializeCharacter(info.id);
        heroList.Add(hero);
        return hero;
    }

    public Hero CreateHero()
    {
        // Create a Random hero
        int randomIndex = Random.Range(0, companionCharacterInfos.Length); 
        return CreateHero(companionCharacterInfos[randomIndex]);
    }
}
