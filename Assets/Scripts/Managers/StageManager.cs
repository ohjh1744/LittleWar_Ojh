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
        //����Ʈ���� �ѹ� �ҷ�����
        LoadSelectedStage(stageName);
    }
	
    private void LoadSelectedStage(string selectedQuestName)
    {
        stageData = Resources.Load<StageData>(selectedQuestName);

        if (stageData != null)
        {
            // �ε� ����
            Debug.Log("QuestData loaded successfully.");
            isLoadedData = true;
            
        }
        else
        {
            // �ε� ����
            Debug.LogError($"Failed to load QuestData for {selectedQuestName}.");
            isLoadedData = false;
        }
    }
}
