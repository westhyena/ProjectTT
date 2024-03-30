using System;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static float WAVE_SUMMON_END_TIME = 10.0f;
    public static float WAVE_SUMMON_INTERVAL = 1.0f;

    public string stageId = "999901";

    StageInfo stageInfo;

    [Serializable]
    public class Wave
    {
        WaveInfo waveInfo;
        CharacterInfo characterInfo;
        int eachCount;
        int createdCount = 0;
        float intervalTimer = 0.0f;

        public Wave(WaveInfo waveInfo)
        {
            this.waveInfo = waveInfo;
            this.characterInfo = DataManager.instance.GetCharacterInfo(waveInfo.monsterId);
            float summonCount = WAVE_SUMMON_END_TIME / WAVE_SUMMON_INTERVAL;
            this.eachCount = Mathf.CeilToInt(waveInfo.totalCount / summonCount);
        }

        void CreateWave(EnemyManager enemyManager, Transform[] spawnPoints)
        {
            int createCount = Math.Min(eachCount, waveInfo.totalCount - createdCount);
            for (int i = 0; i < createCount; ++i)
            {
                enemyManager.CreateEnemy(
                    characterInfo,
                    spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position
                );
            }
            createdCount += createCount;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Transform[] spawnPoints)
        {
            if (createdCount >= waveInfo.totalCount) return true;

            intervalTimer += deltaTime;
            if (createdCount == 0 && intervalTimer > (waveInfo.startTime / 1000.0f))
            {
                // 처음 생성
                intervalTimer -= waveInfo.startTime / 1000.0f;
                CreateWave(enemyManager, spawnPoints);
            }
            else if (createdCount > 0 && intervalTimer > WAVE_SUMMON_INTERVAL)
            {
                intervalTimer -= WAVE_SUMMON_INTERVAL;
                CreateWave(enemyManager, spawnPoints);
            }
            return false;
        }
    }

    [Serializable]
    public class WaveGroup
    {
        WaveGroupInfo info;
        List<Wave> waveList = new();
        float endTime;
        float timer = 0.0f;

        public WaveGroup(WaveGroupInfo info, float endTime)
        {
            this.info = info;

            WaveInfo[] waveInfos = new WaveInfo[]
            {
                DataManager.instance.GetWaveInfo(info.wave01),
                DataManager.instance.GetWaveInfo(info.wave02),
                DataManager.instance.GetWaveInfo(info.wave03),
                DataManager.instance.GetWaveInfo(info.wave04),
            };
            foreach (WaveInfo waveInfo in waveInfos)
            {
                if (waveInfo == null) continue;
                waveList.Add(new Wave(waveInfo));
            }

            this.endTime = endTime;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Transform[] spawnPoints)
        {
            bool isSpawnEnd = true;
            timer += deltaTime;
            foreach (Wave wave in waveList)
            {
                bool eachEnd = wave.Update(deltaTime, enemyManager, spawnPoints);
                if (!eachEnd) isSpawnEnd = false;
            }

            bool killedAll = isSpawnEnd && enemyManager.AliveEnemyList.Count == 0;
            bool timeEnd = timer > endTime;
            return killedAll || timeEnd;
        }
    }

    readonly List<WaveGroup> waveGroupList = new ();

    int curWaveGroupIndex = 0;

    public float[] WaveEndTime = new float[]
    {
        60.0f,
        60.0f,
        60.0f,
        60.0f,
        60.0f
    };

    Stage stage;

    void Awake()
    {
        stageInfo = DataManager.instance.GetStageInfo(stageId);

        GameObject stagePrefab = ResourceManager.GetStagePrefab(stageInfo.stagePrefab);
        GameObject stageObject = Instantiate(stagePrefab);
        this.stage = stageObject.GetComponent<Stage>();

        WaveGroupInfo[] groupInfos = new WaveGroupInfo[]
        {
            DataManager.instance.GetWaveGroupInfo(stageInfo.phase01waveGroup),
            DataManager.instance.GetWaveGroupInfo(stageInfo.phase02waveGroup),
            DataManager.instance.GetWaveGroupInfo(stageInfo.phase03waveGroup),
            DataManager.instance.GetWaveGroupInfo(stageInfo.phase04waveGroup),
            DataManager.instance.GetWaveGroupInfo(stageInfo.phase05waveGroup)
        };

        foreach (WaveGroupInfo groupInfo in groupInfos)
        {            
            if (groupInfo == null) continue;

            waveGroupList.Add(new WaveGroup(groupInfo, WaveEndTime[curWaveGroupIndex]));
        }
    }

    void Start()
    {
        StartWave();
    }

    void StartWave()
    {
        UIManager.instance.waveUI.StartWave(curWaveGroupIndex + 1);
    }

    void CheckWave()
    {
        if (curWaveGroupIndex >= waveGroupList.Count) return;

        WaveGroup waveGroup = waveGroupList[curWaveGroupIndex];
        bool waveEnd = waveGroup.Update(
            Time.deltaTime,
            EnemyManager.instance,
            stage.spawnPoints
        );
        if (waveEnd)
        {
            curWaveGroupIndex++;
            StartWave();
        }
    }

    void Update()
    {
        CheckWave();
    }
}
