using System;
using System.Collections.Generic;
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

    void Awake()
    {
        StageWaveDataElement stageInfo = DataMgr.instance.m_StageWaveDataElementDic[stageId];

        GameObject stagePrefab = ResourceManager.GetStagePrefab(stageInfo.StageMapObjectName);
        GameObject stageObject = Instantiate(stagePrefab);
        this.stage = stageObject.GetComponent<Stage>();

        List<WaveDataInfo>[] groupInfos = new List<WaveDataInfo>[]
        {
            stageInfo.Wave0,
            stageInfo.Wave1,
            stageInfo.Wave2,
            stageInfo.Wave3,
            stageInfo.Wave4,
        };

        foreach (List<WaveDataInfo> groupInfo in groupInfos)
        {
            if (groupInfo == null) continue;

            waveGroupList.Add(new WaveGroup(groupInfo, 60.0f));
        }
    }

    void Start()
    {
        StartWave();
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
        CheckWave();
    }
}
