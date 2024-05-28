using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class MagicTower2 : MagicTowerBase
{
    // 스탯 조정
    private void Awake()
    {
        InitTower(2, 0.7f, 200);
    }

    // 몬스터 처리
    // 매직타워2
    // 매직타워 2는 지속시간이 길지만 감소량이 적음

    // 코루틴
    // protected override void MonsterInteraction()
    // {
    //     Collider2D[] hits = Physics2D.OverlapCircleAll(target.position, 4f);

    //     // 스플래쉬 + 속도감소 처리
    //     foreach (Collider2D hit in hits)
    //     {
    //         if (hit.CompareTag("Enemy"))
    //         {
    //             StartCoroutine(Slow(hit.GetComponent<Enemy>()));
    //         }
    //     }
    // }

    // 유니태스크
    protected override void MonsterInteraction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(target.position, 4f);

        // 스플래쉬 + 속도감소 처리
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Slow(hit.GetComponent<Enemy>()).Forget();
            }
        }
    }

    // 속도감소

    // 코루틴
    // private IEnumerator Slow(Enemy enemy)
    // {
    //     enemy.moveSpeed *= 0.8f;  // 슬로우

    //     yield return new WaitForSeconds(basicDamage);  // 지속시간

    //     enemy.moveSpeed = enemy.originSpeed;  // 해제
    // }

    // 유니태스크
    private async UniTaskVoid Slow(Enemy enemy)
    {
        enemy.moveSpeed *= 0.8f;  // 슬로우

        await UniTask.Delay(TimeSpan.FromSeconds(basicDamage));  // 지속시간

        enemy.moveSpeed = enemy.originSpeed;  // 해제
    }
}
