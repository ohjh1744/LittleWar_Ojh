using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string description = "실제 StageTime = 스폰인터벌 * 스폰 넘 + 2 + [추가 시간]";
    public string stageInfo;  //HUD 업데이트용
    public PoolManager.EnemyType enemyType; //에네미타입정보
    public int stageSpawnInterval;  //스폰 간격
    public int stageSpawnNum;  //스테이지 수 
    public int stageTime; //스테이지 시간
    public int stageEnableSpawnPt; //가능한 스폰 장소
}
