using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
   public class WaveMonsterInfo
    {
        public int monsterIndex;
        public float firstDelay;
        public float interval;
        public int totalCount;
        public int eachCount;

        int createdCount = 0;
        float intervalTimer = 0.0f;

        public WaveMonsterInfo(int monsterIndex, float firstDelay, float interval, int totalCount, int eachCount)
        {
            this.monsterIndex = monsterIndex;
            this.firstDelay = firstDelay;
            this.interval = interval;
            this.totalCount = totalCount;
            this.eachCount = eachCount;
        }

        protected void CreateWave(EnemyManager enemyManager, Transform[] spawnPoints)
        {
            for (int i = 0; i < eachCount; ++i)
            {
                enemyManager.CreateEnemy(spawnPoints[Random.Range(0, spawnPoints.Length)].position);
            }
            createdCount += eachCount;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Transform[] spawnPoints)
        {
            if (createdCount >= totalCount)
            {
                return true;
            }
            intervalTimer += deltaTime;
            if (createdCount == 0 && intervalTimer > firstDelay)
            {
                // 처음 생성
                intervalTimer = 0.0f;
                CreateWave(enemyManager, spawnPoints);
            }
            else if (createdCount > 0 && intervalTimer > interval)
            {
                intervalTimer = 0.0f;
                CreateWave(enemyManager, spawnPoints);
            }
            return false;
        }
    }

    public class WaveInfo
    {
        public List<WaveMonsterInfo> waveMonsterInfoList;
        public WaveInfo(WaveMonsterInfo[] waveMonsterInfos)
        {
            waveMonsterInfoList = new List<WaveMonsterInfo>(waveMonsterInfos);
        }
    }

    public Transform[] spawnPoints;

    EnemyManager enemyManager;
    readonly List<WaveInfo> waveInfoList = new();

    float totalTimer = 0.0f;
    float waveTimer = 0.0f;

    int curWaveIndex = 0;

    void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new (0, 1.0f, 2.0f, 10, 10),
        }));
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new (0, 1.0f, 2.0f, 20, 10),
        }));
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new(0, 1.0f, 2.0f, 30, 10),
        }));
        
    }

    void CheckWave()
    {
        WaveInfo curWaveInfo = waveInfoList[curWaveIndex];
        bool isSpawnEnd = true;
        foreach (WaveMonsterInfo waveMonsterInfo in curWaveInfo.waveMonsterInfoList)
        {
            bool eachEnd = waveMonsterInfo.Update(Time.deltaTime, enemyManager, spawnPoints);
            if (!eachEnd)
            {
                isSpawnEnd = false;
            }
        }

        if (isSpawnEnd && enemyManager.AliveEnemyList.Count == 0)
        {
            curWaveIndex++;
            waveTimer = 0.0f;
        }
    }

    void Update()
    {
        waveTimer += Time.deltaTime;
        totalTimer += Time.deltaTime;
        CheckWave();
    }

}
