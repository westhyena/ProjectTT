using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static float WAVE_SUMMON_END_TIME = 10.0f;
    public static float WAVE_SUMMON_INTERVAL = 1.0f;

    public int stageId = 0;

    StageWaveDataElement stageInfo;

    [Serializable]
    public class Wave
    {
        WaveDataInfo waveInfo;
        CharacterDataElement characterInfo;
        public bool IsBossWave => waveInfo.MonsterType == MonsterType_E.Boss;
        int eachCount;
        int createdCount = 0;
        float intervalTimer = 0.0f;

        public Wave(WaveDataInfo waveInfo)
        {
            this.waveInfo = waveInfo;
            // TODO Level
            this.characterInfo = DataMgr.instance.GetCharacterDataElement(waveInfo.CharacterID);
            if (this.characterInfo == null)
            {
                this.characterInfo = DataMgr.instance.GetCharacterDataElement(1000);
            }

            float summonCount = WAVE_SUMMON_END_TIME / WAVE_SUMMON_INTERVAL;
            this.eachCount = Mathf.CeilToInt(waveInfo.SummonCount / summonCount);
        }

        void CreateWave(EnemyManager enemyManager, Stage stage)
        {
            int createCount = Math.Min(eachCount, waveInfo.SummonCount - createdCount);

            Transform[] spawnPoints = this.IsBossWave ? stage.bossSpawnPoints : stage.spawnPoints;
            if (spawnPoints.Length == 0) spawnPoints = stage.spawnPoints;

            for (int i = 0; i < createCount; ++i)
            {
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle;
                Vector3 spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position;
                enemyManager.CreateEnemy(
                    characterInfo,
                    waveInfo.CharacterLevel,
                    spawnPoint + (Vector3)randomOffset);
            }
            createdCount += createCount;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Stage stage)
        {
            if (createdCount >= waveInfo.SummonCount) return true;

            intervalTimer += deltaTime;
            if (createdCount == 0 && intervalTimer > waveInfo.SummonTime)
            {
                // 처음 생성
                intervalTimer -= waveInfo.SummonTime;
                CreateWave(enemyManager, stage);
            }
            else if (createdCount > 0 && intervalTimer > WAVE_SUMMON_INTERVAL)
            {
                intervalTimer -= WAVE_SUMMON_INTERVAL;
                CreateWave(enemyManager, stage);
            }
            return false;
        }
    }

    [Serializable]
    public class WaveGroup
    {
        // List<WaveDataInfo> waveList = new ();
        WaveGroupInfo info;
        List<Wave> waveList = new();
        public bool IsBossWaveGroup => waveList.Exists(w => w.IsBossWave);
        float endTime;
        public float EndTime => endTime;
        float timer = 0.0f;

        public WaveGroup(List<WaveDataInfo> waveDataList, float endTime)
        {
            foreach (WaveDataInfo waveData in waveDataList)
            {
                waveList.Add(new Wave(waveData));
            }

            this.endTime = endTime;
        }

        public bool Update(float deltaTime, EnemyManager enemyManager, Stage stage)
        {
            bool isSpawnEnd = true;
            timer += deltaTime;
            foreach (Wave wave in waveList)
            {
                bool eachEnd = wave.Update(deltaTime, enemyManager, stage);
                if (!eachEnd) isSpawnEnd = false;
            }

            bool killedAll = isSpawnEnd && enemyManager.AliveEnemyList.Count == 0;
            bool timeEnd = timer > endTime;
            return killedAll || timeEnd;
        }
    }

    readonly List<WaveGroup> waveGroupList = new ();

    int curWaveGroupIndex = 0;

    Stage stage;

    public float TotalTime
    {
        get
        {
            float time = 0.0f;
            foreach (WaveGroup waveGroup in waveGroupList)
            {
                time += waveGroup.EndTime;
            }
            return time;
        }
    }

    public void CreateStage()
    {
        if (this.stage != null)
        {
            Destroy(this.stage.gameObject);
        }
        StageWaveDataElement stageInfo = DataMgr.instance.m_StageWaveDataElementDic[stageId];

        GameObject stagePrefab = ResourceManager.GetStagePrefab(stageInfo.StageMapObjectName);
        GameObject stageObject = Instantiate(stagePrefab);
        this.stage = stageObject.GetComponent<Stage>();

        List<SpotPointDataInfo>[] spotInfos = new List<SpotPointDataInfo>[]
        {
            stageInfo.Spot0,
            stageInfo.Spot1,
            stageInfo.Spot2,
            stageInfo.Spot3,
            stageInfo.Spot4,
            stageInfo.Spot5,
            stageInfo.Spot6,
            stageInfo.Spot7,
            stageInfo.Spot8,
            stageInfo.Spot9
        };

        for (int i = 0; i < spotInfos.Length; ++i)
        {
            List<SpotPointDataInfo> spotInfo = spotInfos[i];
            if (spotInfo == null) continue;

            if (i >= this.stage.spawnPoints.Length) break;

            Transform spawnPoint = this.stage.spawnPoints[i];
            foreach (SpotPointDataInfo spotData in spotInfo)
            {
                for (int j = 0; j < spotData.SummonCount; ++j)
                {
                    Vector2 randomOffset = UnityEngine.Random.insideUnitCircle;
                    EnemyManager.instance.CreateEnemy(
                        DataMgr.instance.GetCharacterDataElement(spotData.CharacterID),
                        spotData.CharacterLevel,
                        spawnPoint.position + (Vector3)randomOffset
                    );
                }
            }
        }
    }

    void Start()
    {
        CreateStage();
    }

    void StartWave()
    {
        if (waveGroupList[curWaveGroupIndex].IsBossWaveGroup)
        {
            UIManager.instance.bossIncomingUI.SetActive(true);
        }
        else 
        {
            UIManager.instance.waveUI.StartWave(curWaveGroupIndex + 1);
        }
    }

    void CheckWave()
    {
        if (curWaveGroupIndex >= waveGroupList.Count) return;

        WaveGroup waveGroup = waveGroupList[curWaveGroupIndex];
        bool waveEnd = waveGroup.Update(
            Time.deltaTime,
            EnemyManager.instance,
            stage
        );
        if (waveEnd)
        {
            curWaveGroupIndex++;
            StartWave();
        }
    }

    void Update()
    {
        // CheckWave();
    }
}
