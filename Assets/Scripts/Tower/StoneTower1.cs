using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class StoneTower1 : TowerBase
{
    // 풀링한 불
    [HideInInspector] public List<GameObject> FirePrefabs = new List<GameObject>();

    // 스탯 조정
    private void Awake()
    {
        InitTower(1, 1.2f, 200);
    }

    // 타겟 공격
    // 스톤타워1
    // 지속딜 불 생성, 해제 불가능한 도트딜 부여

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

    //         // 무기 발사
    //         Shot();

    //         for(int i = 0; i < 3; i++)
    //         {
    //             // 사운드
    //             SoundManager.Instance.PlaySFX(soundType);

    //             // 잠시 대기 후
    //             yield return halfSeconds;
    //         }

    //         // 스톤 타워 1 불 초기화
    //         StoneTowerFireInit();
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

            // 무기 발사
            Shot();

            for(int i = 0; i < 3; i++)
            {
                // 사운드
                SoundManager.Instance.PlaySFX(soundType);

                // 잠시 대기 후
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tok);
            }

            // 스톤 타워 1 불 초기화
            StoneTowerFireInit();
        }
    }

    // 무기 발사
    protected override void Shot()
    {
        // 타워 무기 가져오기
        GameObject towerWeapon = PoolManager.Instance.GetTowerWeapon(towerWeaponType);
        FirePrefabs.Add(towerWeapon);

        // 데미지 설정
        towerWeapon.GetComponent<Fire>().fireDamage = basicDamage;

        // 위치 및 회전 초기화
        towerWeapon.transform.position = target.transform.position;
        towerWeapon.transform.rotation = towerWeapon.transform.rotation;
    }

    // 스톤 타워 1 불 초기화
    private void StoneTowerFireInit()
    {
        for(int i = 0; i < FirePrefabs.Count; i++) FirePrefabs[i].gameObject.SetActive(false);

        FirePrefabs.Clear();
    }
}
