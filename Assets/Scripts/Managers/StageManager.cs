using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonBehaviour<StageManager>
{
    public StageData stageData;
    public bool isLoadedData;
    
    public void LoadStageData(string stageName)
    {   
        isLoadedData = false;
        //퀘스트에셋 넘버 불러오기
        LoadSelectedStage(stageName);
    }
	
    private void LoadSelectedStage(string selectedQuestName)
    {
        stageData = Resources.Load<StageData>(selectedQuestName);

        if (stageData != null)
        {
            // 로드 성공
            Debug.Log("QuestData loaded successfully.");
            isLoadedData = true;
            
        }
        else
        {
            // 로드 실패
            Debug.LogError($"Failed to load QuestData for {selectedQuestName}.");
            isLoadedData = false;
        }
    }
}
