using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;

    public Player player;
    public Transform enemyRoot;
    public float respawnTime = 5.0f;
    public int respawnCount = 5;

    public float respawnDistance = 20.0f;

    float respawnTimer = 0.0f;

    void Update()
    {
        respawnTimer += Time.deltaTime;
        if (respawnTimer > respawnTime)
        {
            respawnTimer = 0.0f;
            for (int i = 0; i < respawnCount; ++i)
            {
                Vector2 randomPosition = respawnDistance * Random.insideUnitCircle.normalized;
                Vector3 position = player.transform.position + new Vector3(
                    randomPosition.x,
                    randomPosition.y,
                    0.0f
                );
                GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
                enemy.transform.parent = enemyRoot;
                enemy.GetComponent<Enemy>().Initialize(player);
            }
        }
    }
}
