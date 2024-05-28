using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class StoneTower2 : TowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(20, 1.2f, 200);
    }

    // 타겟 공격
    // 스톤타워2
    // 길을 막는 돌 생성

    // 코루틴
    // protected override IEnumerator Attack()
    // {
    //     while (isTarget)
    //     {
    //         // 공격속도만큼 대기
    //         yield return new WaitForSeconds(attackSpeed);

    //         // 애니메이션
    //         towerAnim[0].SetTrigger("atkTrig");

    //         // 잠시 대기 후
    //         yield return halfSeconds;

    //         // 발사
    //         Shot();

    //         // 사운드
    //         SoundManager.Instance.PlaySFX(soundType);
    //     }
    // }

    // 유니태스크
    protected override async UniTaskVoid Attack(CancellationToken tok)
    {
        while (!tok.IsCancellationRequested && isTarget)
        {
            // 공격속도만큼 대기
            await UniTask.Delay(TimeSpan.FromSeconds(attackSpeed), cancellationToken: tok);

            // 애니메이션
            towerAnim[0].SetTrigger("atkTrig");

            // 잠시 대기 후
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tok);

            // 발사
            Shot();

            // 사운드
            SoundManager.Instance.PlaySFX(soundType);
        }
    }

    // 무기 발사
    protected override void Shot()
    {
        // 타워 무기 가져오기
        GameObject towerWeapon = PoolManager.Instance.GetTowerWeapon(towerWeaponType);
        towerWeapon.GetComponent<Stone>().stoneHP = basicDamage;

        // 위치 및 회전 초기화
        towerWeapon.transform.position = target.transform.position;
        towerWeapon.transform.rotation = towerWeapon.transform.rotation;
    }
}
