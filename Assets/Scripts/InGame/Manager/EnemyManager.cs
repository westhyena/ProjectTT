using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Player player;
    public Transform enemyRoot;
    readonly List<Enemy> enemyList = new();
    public List<Enemy> EnemyList => enemyList;
    public int enemyMax = 10;

    public float respawnTime = 5.0f;
    public int respawnCount = 5;

    public float respawnDistance = 20.0f;

    float respawnTimer = 0.0f;

    void Update()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > respawnTime && enemyList.Count < enemyMax)
        {
            respawnTimer = 0.0f;
            for (int i = 0; i < respawnCount; ++i)
            {
                if (enemyList.Count >= enemyMax)
                {
                    break;
                }
                Vector2 randomPosition = respawnDistance * Random.insideUnitCircle.normalized;
                Vector3 position = player.transform.position + new Vector3(
                    randomPosition.x,
                    randomPosition.y,
                    0.0f
                );
                GameObject enemyObj = Instantiate(enemyPrefab, position, Quaternion.identity);
                enemyObj.transform.parent = enemyRoot;
                enemyObj.transform.localRotation = Quaternion.Euler(GameManager.instance.characterRotation);
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                enemy.Initialize(player);
                enemyList.Add(enemy);
            }
        }
    }
}
