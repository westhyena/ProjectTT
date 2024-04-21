using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;
    public static EnemyManager instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = FindObjectOfType<EnemyManager>();
            }
            return _instance;
        }
    }

    public GameObject enemyPrefab;

    public Transform enemyRoot;

    readonly Dictionary<int, Enemy> enemyMap = new();
    public List<Enemy> EnemyList => enemyMap.Values.ToList();
    public List<Enemy> AliveEnemyList => EnemyList.FindAll(enemy => !enemy.IsDead);

    public Enemy CreateEnemy(CharacterDataElement info, Vector3 position)
    {
        Player player = GameManager.instance.Player;
        GameObject prefab = ResourceManager.GetCharacterPrefab(info.ObjectFileName);

        GameObject enemyObj = Instantiate(prefab, position, Quaternion.identity);
        enemyObj.transform.parent = enemyRoot;
        enemyObj.transform.localRotation = Quaternion.Euler(GameManager.instance.characterRotation);
        Enemy enemy = enemyObj.GetOrAddComponent<Enemy>();
        enemy.Initialize(player);
        enemy.InitializeCharacter(info.ID);
        enemyMap[enemy.GetInstanceID()] = enemy;
        return enemy;
    }

    public void DestroyEnemy(Enemy enemy)
    {
        enemyMap.Remove(enemy.GetInstanceID());
        Destroy(enemy.gameObject);
    }
}
