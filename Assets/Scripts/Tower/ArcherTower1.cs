using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherTower1 : ArcherTowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(50, 1.0f, 100);
    }
}
