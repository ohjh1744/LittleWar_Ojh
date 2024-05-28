using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldManager : SingletonBehaviour<GoldManager>
{
    [SerializeField] public int playerGold;
    [SerializeField] TMP_Text totalGold;
    void Start()
    {
        playerGold = 250;
        InitUIGold();
    }

    public void InitUIGold()
    {
        totalGold.text = playerGold.ToString();
    }
    //��� ȹ�� �Լ�
    public void AcquireGold(int amount)
    {   //Enemy�� �׾ ��Ȱ��ȭ��ų�� �Լ� ȣ���ϸ��.
        playerGold += amount;
        InitUIGold();
    }

    public bool UseGold(int amount)
    {
        // ����Ϸ��� ���� �÷��̾��� �����ݺ��� �۰ų� ������ Ȯ��
        if (amount <= playerGold)
        {
            playerGold -= amount; // �� ���
            InitUIGold();
            return true;
        }
        InitUIGold();
        return false;
    }
}