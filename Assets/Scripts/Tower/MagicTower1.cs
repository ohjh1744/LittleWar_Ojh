using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTower1 : MagicTowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(50, 1.2f, 150);
    }
}
