using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleTower : MonoBehaviour
{
    [SerializeField] private TowerBase towerBase; // 타워 베이스

    // 성에 설치되어있는 아쳐타워 4 공격력 조정
    private void Start() { towerBase.basicDamage = 500; }
}
