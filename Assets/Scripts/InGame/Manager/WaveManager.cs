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
        float waveEndTime = 60.0f;
        float waveTimer = 0.0f;

        public WaveInfo(WaveMonsterInfo[] waveMonsterInfos, float waveEndTime)
        {
            waveMonsterInfoList = new List<WaveMonsterInfo>(waveMonsterInfos);
            this.waveEndTime = waveEndTime;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Transform[] spawnPoints)
        {
            bool isSpawnEnd = true;
            foreach (WaveMonsterInfo waveMonsterInfo in waveMonsterInfoList)
            {
                bool eachEnd = waveMonsterInfo.Update(deltaTime, enemyManager, spawnPoints);
                if (!eachEnd)
                {
                    isSpawnEnd = false;
                }
            }

            return (
                (isSpawnEnd && enemyManager.AliveEnemyList.Count == 0)
                || waveTimer > waveEndTime
            );
        }
    }

    public Transform[] spawnPoints;

    EnemyManager enemyManager;
    readonly List<WaveInfo> waveInfoList = new();

    float totalTimer = 0.0f;

    int curWaveIndex = 0;

    void Awake()
    {
        enemyManager = GetComponent<EnemyManager>();
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new (0, 1.0f, 2.0f, 10, 10),
        }, 60.0f));
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new (0, 1.0f, 2.0f, 20, 10),
        }, 60.0f));
        waveInfoList.Add(new WaveInfo(new WaveMonsterInfo[] {
            new(0, 1.0f, 2.0f, 30, 10),
        }, 60.0f));
    }

    void CheckWave()
    {
        if (curWaveIndex >= waveInfoList.Count)
        {
            return;
        }
        WaveInfo curWaveInfo = waveInfoList[curWaveIndex];
        bool waveEnd = curWaveInfo.Update(Time.deltaTime, enemyManager, spawnPoints);
        if (waveEnd)
        {
            curWaveIndex++;
        }
    }

    void Update()
    {
        totalTimer += Time.deltaTime;
        CheckWave();
    }

}
