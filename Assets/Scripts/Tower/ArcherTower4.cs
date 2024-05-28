using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ArcherTower4 : ArcherTowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(700, 1.2f, 800);
    }

    // 타겟 공격
    // 아쳐타워4
    // 광역공격

    // 코루틴
    // protected override IEnumerator Attack()
    // {
    //     while (isTarget)
    //     {
    //         // 공격속도만큼 대기
    //         yield return new WaitForSeconds(attackSpeed);

    //         // 애니메이션
    //         for(int i = 0; i < towerAnim.Count; i++) towerAnim[i].SetTrigger("atkTrig");

    //         // 사운드
    //         SoundManager.Instance.PlaySFX(soundType);

    //         // 싱크
    //         yield return halfSeconds;

    //         // 발사
    //         Shot();
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
            for(int i = 0; i < towerAnim.Count; i++) towerAnim[i].SetTrigger("atkTrig");

            // 사운드
            SoundManager.Instance.PlaySFX(soundType);

            // 싱크
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: tok);

            // 발사
            Shot();
        }
    }

    // 몬스터 처리
    protected override void MonsterInteraction()
    {
        // 광역 처리
        Collider2D[] hits = Physics2D.OverlapCircleAll(target.position, 6f);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy")) hit.GetComponent<Enemy>().hp -= basicDamage;
        }
    }
}
