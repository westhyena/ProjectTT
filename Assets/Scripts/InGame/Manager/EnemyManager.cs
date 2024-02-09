using System.Collections;
using System.Collections.Generic;
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

    public Player player;
    public Transform enemyRoot;
    readonly List<Enemy> enemyList = new();
    public List<Enemy> EnemyList => enemyList;
    public List<Enemy> AliveEnemyList => enemyList.FindAll(enemy => !enemy.IsDead);

    public void CreateEnemy(Vector3 position)
    {
        GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemyObj.transform.parent = enemyRoot;
        enemyObj.transform.localRotation = Quaternion.Euler(GameManager.instance.characterRotation);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.Initialize(player);
        enemyList.Add(enemy);
    }
}
