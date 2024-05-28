using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageTimer : MonoBehaviour
{
    public Image waveTimeImage;
    private int _thisStageTime;  //현재 스테이지 시간

    private void OnEnable()
    {
        waveTimeImage.fillAmount = 0f;
        _thisStageTime = StageManager.Instance.stageData.stageTime; //스테이지시간 로드
    }
    
    void Update()
    {
        waveTimeImage.fillAmount += (Time.deltaTime/_thisStageTime);
        if(waveTimeImage.fillAmount >= 0.99){
            EnemySpawner.Instance.isCurWaveEnded = true; //현재스테이지 종료
            gameObject.SetActive(false);
        }
    }
}
