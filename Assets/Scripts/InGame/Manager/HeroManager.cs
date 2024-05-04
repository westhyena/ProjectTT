using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    CharacterDataElement[] companionCharacterInfos;
    public CharacterDataElement[] CompanionCharacterInfos => companionCharacterInfos;

    public Transform companionRoot;

    readonly Dictionary<int, Hero> heroMap = new();
    public List<Hero> HeroList => heroMap.Values.ToList();
    public List<Hero> AliveHeroList => HeroList.FindAll(hero => !hero.IsDead);

    public float spawnRange = 5.0f;

    void Start()
    {
        int[] companionCharacterIds = GameManager.instance.companionCharacterIds;
        companionCharacterInfos = new CharacterDataElement[companionCharacterIds.Length];
        for (int i = 0; i < companionCharacterIds.Length; ++i)
        {
            companionCharacterInfos[i] = DataMgr.instance.GetCharacterDataElement(companionCharacterIds[i]);
        }
    }

    public Player CreatePlayer(int playerCharacterId)
    {
        CharacterDataElement info = DataMgr.instance.GetCharacterDataElement(playerCharacterId);
        GameObject prefab = ResourceManager.GetCharacterPrefab(info.ObjectFileName);
        GameObject playerObj = Instantiate(
            prefab,
            Vector3.zero,
            Quaternion.Euler(GameManager.instance.characterRotation)
        );
        Player player = playerObj.AddComponent<Player>();
        player.InitializeCharacter(playerCharacterId, 0);
        return player;
    }

    Hero CreateHero(CharacterDataElement info)
    {
        Player player = GameManager.instance.Player;
        GameObject prefab = ResourceManager.GetCharacterPrefab(info.ObjectFileName);
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
        Hero hero = heroObj.GetOrAddComponent<Hero>();
        hero.Initialize(player);
        hero.InitializeCharacter(info.ID, 0);
        heroMap[hero.GetInstanceID()] = hero;
        return hero;
    }

    public Hero CreateHero()
    {
        // Create a Random hero
        int randomIndex = Random.Range(0, companionCharacterInfos.Length); 
        return CreateHero(companionCharacterInfos[randomIndex]);
    }
}
