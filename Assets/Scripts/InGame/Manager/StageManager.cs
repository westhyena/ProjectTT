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
        public bool IsBossWave => characterInfo.charType == "BOSS";
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

        void CreateWave(EnemyManager enemyManager, Stage stage)
        {
            int createCount = Math.Min(eachCount, waveInfo.totalCount - createdCount);
            
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
            if (createdCount >= waveInfo.totalCount) return true;

            intervalTimer += deltaTime;
            if (createdCount == 0 && intervalTimer > (waveInfo.startTime / 1000.0f))
            {
                // 처음 생성
                intervalTimer -= waveInfo.startTime / 1000.0f;
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
        WaveGroupInfo info;
        List<Wave> waveList = new();
        public bool IsBossWaveGroup => waveList.Exists(w => w.IsBossWave);
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

        float waveEnd1 = float.Parse(DataManager.instance.GetConstValue("BATTLE_PHASE_DURATION_2")) / 1000f;
        float waveEnd2 = float.Parse(DataManager.instance.GetConstValue("BATTLE_PHASE_DURATION_3")) / 1000f;
        float waveEnd3 = float.Parse(DataManager.instance.GetConstValue("BATTLE_PHASE_DURATION_4")) / 1000f;
        float waveEnd4 = float.Parse(DataManager.instance.GetConstValue("BATTLE_PHASE_DURATION_5")) / 1000f;
        float waveEnd5 = (
            float.Parse(DataManager.instance.GetConstValue("BATTEL_STAGE_DURATION_TIME"))
            - waveEnd1
            - waveEnd2
            - waveEnd3
            - waveEnd4
        );

        float[] waveEndTimes = new float[]
        {
            waveEnd1,
            waveEnd2,
            waveEnd3,
            waveEnd4,
            waveEnd5
        };

        foreach (WaveGroupInfo groupInfo in groupInfos)
        {            
            if (groupInfo == null) continue;

            waveGroupList.Add(new WaveGroup(groupInfo, waveEndTimes[curWaveGroupIndex]));
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
