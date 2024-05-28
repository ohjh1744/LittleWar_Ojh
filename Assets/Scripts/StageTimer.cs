using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageTimer : MonoBehaviour
{
    public Image waveTimeImage;
    private int _thisStageTime;  //���� �������� �ð�

    private void OnEnable()
    {
        waveTimeImage.fillAmount = 0f;
        _thisStageTime = StageManager.Instance.stageData.stageTime; //���������ð� �ε�
    }
    
    void Update()
    {
        waveTimeImage.fillAmount += (Time.deltaTime/_thisStageTime);
        if(waveTimeImage.fillAmount >= 0.99){
            EnemySpawner.Instance.isCurWaveEnded = true; //���罺������ ����
            gameObject.SetActive(false);
        }
    }
}
