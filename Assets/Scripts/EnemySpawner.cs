using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemySpawner : SingletonBehaviour<EnemySpawner>
{
    public TextMeshProUGUI stageBreakSec;
    public TextMeshProUGUI thisStageInfo;
    public GameObject stageTimerImage; 
    [SerializeField] private Transform[] spawnPoints;
    private int _thisStageSpawnInterval; //해당 스테이지 스폰 간격
    private int _thisStageEnableSpawnPt;  //해당 스테이지 가능한 스폰 포인트 수
    private int _thisStageEnemyNum; //해당 스테이지 에네미 수
    private readonly string _stageName = "StageData";
    private int _thisStageNum = 1;
    public bool isCurWaveEnded = false;

    private void Start()
    {
        StartGame().Forget(); //게임시작하면 호출
    }
    private async UniTaskVoid StartGame()
    {
        while (_thisStageNum != 11) //임시 11스테이지까지
        {
            isCurWaveEnded = false;
            CountDownAndSpawn().Forget(); //10초 대기
            await UniTask.WaitUntil(()=> isCurWaveEnded); //해당 웨이브가 끝날떄까지 대기
            _thisStageNum++; //다음 스테이지로 (변수증가)
        }
    }
    
    private void InitStageData()  //해당 스테이지 데이터를 초기화
    {
        StageManager.Instance.LoadStageData(_stageName + _thisStageNum); //스테이지정보 로드
        
        if (!StageManager.Instance.isLoadedData) return; //데이터 로드 오류
        thisStageInfo.text = StageManager.Instance.stageData.stageInfo; //HUD wave 업데이트
        
        //각각 해당 스테이지에 대한 정보 업데이트
        _thisStageSpawnInterval = StageManager.Instance.stageData.stageSpawnInterval; 
        _thisStageEnemyNum = StageManager.Instance.stageData.stageSpawnNum; 
        _thisStageEnableSpawnPt = StageManager.Instance.stageData.stageEnableSpawnPt; 
        
    }

    private async UniTaskVoid CountDownAndSpawn()
    {
        isCurWaveEnded = false;
        //10초를 대기하며 HUD 남은 초 수 업데이트
        stageBreakSec.gameObject.SetActive(true);
        int cnt = 10;
        while (cnt > 0)
        {   
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            cnt--;
            stageBreakSec.text = cnt.ToString();
        }
        stageBreakSec.text = "10";
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        
        stageBreakSec.gameObject.SetActive(false);
        
        ActiveWaveStage().Forget(); //다음스테이지 활성화

    }
   
    private async UniTaskVoid ActiveWaveStage()
    {
        InitStageData(); //해당 스테이지에대한 스크립터블 데이터 초기화
        stageTimerImage.SetActive(true); //해당스테이지 타이머 작동

        // 배경음 교체
        if(StageManager.Instance.stageData.enemyType == PoolManager.EnemyType.MiddleBoss) SoundManager.Instance.PlayBGM(SoundType.보스BGM3);
        else if(StageManager.Instance.stageData.enemyType == PoolManager.EnemyType.LastBoss) SoundManager.Instance.PlayBGM(SoundType.보스BGM2);
        else if(StageManager.Instance.stageData.enemyType == PoolManager.EnemyType.Enemy6) SoundManager.Instance.PlayBGM(SoundType.일반BGM);
        
        for (int i = 0; i < _thisStageEnemyNum; i++)  //해당스테이지 에네미넘만큼 반복 
        {
            //해당스테이지에대해 가능한 스폰포인트수를 조사
            _thisStageEnableSpawnPt = StageManager.Instance.stageData.stageEnableSpawnPt;
            while (_thisStageEnableSpawnPt >= 0)
            {   
                //각각스폰포인트수에 모두 스폰
                SpawnEnemyInSp(_thisStageEnableSpawnPt);
                _thisStageEnableSpawnPt--;
            }
            //해당 스테이지 스폰간격만큼 대기 후 다시 반복
            await UniTask.Delay(TimeSpan.FromSeconds(_thisStageSpawnInterval));
            
        }
        //모두 스폰되면 2초 대기
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
    }
    
    private void SpawnEnemyInSp(int sp)
    {
        Enemy enemy = PoolManager.Instance.GetEnemy(StageManager.Instance.stageData.enemyType).GetComponent<Enemy>();
        enemy.gameObject.transform.position = (spawnPoints[sp].position);
        enemy.movePointNum = sp + 1; 
        enemy.FirstMoveTarget();

        if(enemy.enemyGold == 2000 || enemy.enemyGold == 10000)
        {
            GameManager.Instance.bossEnemy = enemy;
            GameManager.Instance.isBossStage = true;
            GameManager.Instance.bossHpPanel.SetActive(true);

            if(enemy.enemyGold == 10000) GameManager.Instance.lastBossImage.sprite = GameManager.Instance.lastBossSprite;
        }
    }
}
